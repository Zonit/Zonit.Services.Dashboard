using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Themes.Services;

/// <summary>
/// Default <see cref="IThemeManager"/>. Lifetime: scoped (one per Blazor Server
/// circuit). Hydrates from the dashboard cookie + system color-scheme media query
/// on first interactive render; persists subsequent changes back to the cookie.
/// </summary>
/// <remarks>
/// <para><b>Cookie names.</b> Two cookies, each 365 d. lifetime:</para>
/// <list type="bullet">
///   <item><c>ui.theme</c> — value = <see cref="IDashboardTheme.Id"/>.</item>
///   <item><c>ui.mode</c> — value = <see cref="ThemeMode"/> stringified.</item>
/// </list>
///
/// <para><b>Resolved on the server, once.</b> <see cref="ThemeMode.Auto"/> is answered from the
/// <c>ui.scheme</c> cookie, which the pre-boot script in <c>DashboardHead</c> writes before the
/// request that reads it. Nothing re-asks the browser afterwards: this class used to query
/// <c>prefers-color-scheme</c> over JS interop during hydration and overwrite the server-side
/// answer, which is exactly what made a page render dark and then repaint light a second later
/// whenever the cookie and the live media query disagreed.</para>
/// </remarks>
internal sealed class ThemeManager : IThemeManager
{
    // Cookie keys. Public consts so external tooling (admin diagnostics) can inspect them
    // without re-declaring the names.
    //
    // Renamed in 10.0.0-preview.11 to drop the framework name. Cookie names are visible to
    // anyone who opens developer tools, and a recognisable one tells an attacker exactly which
    // open-source stack — and often which version — to go looking for advisories against. The
    // cost of the rename is one reset of each user's stored preference on upgrade: the old
    // cookies are simply not read, the theme falls back to its default once, and the next change
    // persists under the new name. That is a fair trade for not advertising the stack.
    public const string ThemeCookieKey = "ui.theme";
    public const string ModeCookieKey  = "ui.mode";
    public const string SystemDarkCookieKey = "ui.scheme";

    private readonly ICookieProvider _cookies;

    // Cached system preference. Populated either from the SystemDarkCookieKey
    // cookie (set by the inline script in DashboardApp.razor before Blazor boots)
    // or, on first visit, from JSRuntime during HydrateAsync. The cookie path is
    // critical for zero-flicker: SSR can produce the correct dark/light HTML on
    // every second+ visit instead of always rendering light then re-rendering dark.
    private bool _systemPrefersDark;

    /// <summary>System preference, resolved through the same gate as <see cref="Mode"/>.</summary>
    private bool SystemPrefersDark { get { EnsureResolved(); return _systemPrefersDark; } }

    public ThemeManager(IEnumerable<IDashboardTheme> themes, ICookieProvider cookies)
    {
        _cookies = cookies;

        // Preserve registration order. The first registered theme is the default
        // when no cookie is present — this is why AddDashboard registers Default
        // first.
        Available = themes.ToArray();
        if (Available.Count == 0)
            throw new InvalidOperationException(
                "No IDashboardTheme registered. AddDashboard() seeds three built-ins; " +
                "verify it ran before resolving IThemeManager.");

        // Backing fields directly: the compiler does not track definite assignment through a
        // property setter, and these are non-nullable.
        _current = Available[0];
        _mode = ThemeMode.Auto;

        // Deliberately NOT reading cookies here. See EnsureResolved.
        TryResolve();
    }

    // Whether the values below were resolved from a cookie jar that actually had something in it.
    // Until that happens they are provisional and get re-resolved on every read.
    private bool _resolved;

    private IDashboardTheme _current;
    private ThemeMode _mode;

    /// <summary>
    /// The active theme. Reading it resolves state from cookies if that has not yet succeeded.
    /// </summary>
    public IDashboardTheme Current { get { EnsureResolved(); return _current; } private set => _current = value; }

    /// <inheritdoc cref="Current"/>
    public ThemeMode Mode { get { EnsureResolved(); return _mode; } private set => _mode = value; }

    public IReadOnlyList<IDashboardTheme> Available { get; }

    /// <summary>
    /// Resolves theme / mode / system-preference from cookies, but only <em>latches</em> the
    /// result once the cookie jar was non-empty.
    /// </summary>
    /// <remarks>
    /// <para><b>This is what fixed the dark-then-light flash on every page load.</b> The
    /// constructor used to read cookies once and keep whatever it got. During SSR that is fine —
    /// the request-scoped jar is seeded from <c>HttpContext.Request.Cookies</c> before anything
    /// resolves this service. In the CIRCUIT it was not: the jar is seeded by
    /// <c>WebsiteHydrator</c>, which <c>AppBase</c> places in the BODY, and <c>DashboardHead</c>
    /// is a HEAD component — built first. So the circuit constructed this manager against an
    /// empty jar, latched <c>Mode = Auto</c> with no system preference (i.e. light), rendered the
    /// page light, and only then did <c>HydrateAsync</c> re-read the cookies and repaint. Server
    /// dark, circuit light, one visible flip per refresh.</para>
    ///
    /// <para>An empty jar is not an answer, it is the absence of one. Treating it as provisional
    /// makes the outcome independent of which component happens to resolve this service first —
    /// the read that matters is the one the layout makes, and by then the jar is populated.</para>
    /// </remarks>
    private void EnsureResolved()
    {
        if (_resolved) return;
        TryResolve();
    }

    private void TryResolve()
    {
        // GetCookies() is the live jar, not a copy — an empty one means "not seeded yet".
        if (_cookies.GetCookies() is not { Count: > 0 }) return;

        ReadFromCookies();
        _resolved = true;
    }

    public bool IsDark => Mode switch
    {
        ThemeMode.Light => false,
        ThemeMode.Dark  => true,
        _               => SystemPrefersDark, // Auto
    };

    public event Action? OnChange;

    public async Task HydrateAsync()
    {
        // ---- 1. Bridge the request → circuit scope split for cookies. ----
        // RefreshAsync re-reads document.cookie via JSInterop — strictly necessary
        // only if a new cookie was set in *another* tab during this circuit's life,
        // because the ctor already seeded from HttpContext.Request.Cookies during
        // SSR. We still call it for consistency and to pick up any cookies the
        // user changed via DevTools mid-session.
        try
        {
            await _cookies.RefreshAsync();
        }
        catch (InvalidOperationException)
        {
            // No JS interop available (prerender). Not fatal — we already have the
            // SSR-time values from the ctor.
        }

        var changed = ReadFromCookies();

        // RefreshAsync read the browser's actual document.cookie, so whatever came back is the
        // truth even when it is empty — latch it and stop re-resolving on every read.
        _resolved = true;

        // ---- 2. No live re-query. ----
        //
        // This used to ask the browser for prefers-color-scheme over JS interop and overwrite
        // whatever the server had resolved. That is precisely what produced the flash: the server
        // renders from the ui.scheme cookie, the circuit answered from the live media query, and
        // whenever the two disagreed the page visibly repainted a second after it appeared.
        //
        // The cookie is no longer allowed to disagree — the pre-boot script in DashboardHead now
        // corrects it before the server renders — so re-asking here can only reintroduce a race
        // it cannot win. The server's answer stands for the lifetime of the page, and a system
        // preference changed mid-session is picked up on the next navigation.
        if (changed)
            OnChange?.Invoke();
    }

    /// <summary>
    /// Pulls theme / mode / system-dark from the cookie repository into the
    /// in-memory state. Returns <see langword="true"/> when something actually
    /// changed — callers raise <see cref="OnChange"/> only if it did.
    /// </summary>
    /// <remarks>
    /// Reads and writes the backing fields directly, never the properties. The properties resolve
    /// lazily through <see cref="EnsureResolved"/>, and this method runs INSIDE that resolution —
    /// touching a property here would re-enter it and recurse until the stack ran out.
    /// </remarks>
    private bool ReadFromCookies()
    {
        var changed = false;

        var themeCookie = _cookies.Get(ThemeCookieKey)?.Value;
        if (!string.IsNullOrWhiteSpace(themeCookie))
        {
            var picked = Available.FirstOrDefault(
                t => string.Equals(t.Id, themeCookie, StringComparison.OrdinalIgnoreCase));
            if (picked is not null && !ReferenceEquals(picked, _current))
            {
                _current = picked;
                changed = true;
            }
        }

        var modeCookie = _cookies.Get(ModeCookieKey)?.Value;
        if (Enum.TryParse<ThemeMode>(modeCookie, ignoreCase: true, out var parsed) && parsed != _mode)
        {
            _mode = parsed;
            changed = true;
        }

        // System dark preference, from the cookie the pre-boot script in DashboardHead writes
        // before Blazor boots. "1" / "0" to keep it one byte.
        //
        // ADOPTED ONCE PER PAGE, and that is the whole fix for the Auto-mode flash.
        //
        // The script writes this cookie on every load — AFTER the server has already rendered
        // from whatever the previous load left behind. HydrateAsync then calls RefreshAsync,
        // which re-reads document.cookie and hands this method the value written moments ago. So
        // in Auto mode the circuit would learn a system preference the server never saw, disagree
        // with the HTML already on screen, and repaint.
        //
        // Measured on WebKit (the iOS engine) at an iPhone profile: first visit with the system
        // in dark rendered light and flipped to dark at 91ms; a stale cookie with the system in
        // light rendered dark and flipped to light at 148ms. Explicit Light and Dark were stable
        // in the same runs, because neither consults this value at all.
        //
        // The server's answer therefore stands for the lifetime of the page. The cookie written
        // this load is for the NEXT request, which is the only place it can be applied without
        // contradicting pixels the visitor is already looking at.
        //
        // That script also reloads once when it finds the cookie disagreed with the real
        // preference, so by the time this read happens the value is the truth rather than the
        // previous visit's guess. A missing cookie still falls back to light — that is the very
        // first request of the very first visit, and the script corrects it before first paint.
        if (!_resolved)
        {
            var systemDark = _cookies.Get(SystemDarkCookieKey)?.Value == "1";
            if (systemDark != _systemPrefersDark)
            {
                _systemPrefersDark = systemDark;
                changed = true;
            }
        }

        return changed;
    }

    public async Task SetThemeAsync(string themeId)
    {
        var picked = Available.FirstOrDefault(
            t => string.Equals(t.Id, themeId, StringComparison.OrdinalIgnoreCase));
        if (picked is null || ReferenceEquals(picked, Current))
            return;

        Current = picked;
        await _cookies.SetAsync(ThemeCookieKey, picked.Id, TimeSpan.FromDays(365));
        OnChange?.Invoke();
    }

    public async Task SetModeAsync(ThemeMode mode)
    {
        if (mode == Mode) return;

        Mode = mode;
        await _cookies.SetAsync(ModeCookieKey, mode.ToString(), TimeSpan.FromDays(365));
        OnChange?.Invoke();
    }
}
