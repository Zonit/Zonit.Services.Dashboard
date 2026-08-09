using Microsoft.JSInterop;
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
/// <para><b>JS interop.</b> <see cref="HydrateAsync"/> queries
/// <c>window.matchMedia('(prefers-color-scheme: dark)').matches</c> to resolve
/// <see cref="ThemeMode.Auto"/>. Silently catches <c>InvalidOperationException</c>
/// raised during prerender (no JS runtime available) — the resolved value stays at
/// its server-side default until the interactive circuit completes hydration.</para>
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
    private readonly IJSRuntime _js;

    // Cached system preference. Populated either from the SystemDarkCookieKey
    // cookie (set by the inline script in DashboardApp.razor before Blazor boots)
    // or, on first visit, from JSRuntime during HydrateAsync. The cookie path is
    // critical for zero-flicker: SSR can produce the correct dark/light HTML on
    // every second+ visit instead of always rendering light then re-rendering dark.
    private bool _systemPrefersDark;

    public ThemeManager(IEnumerable<IDashboardTheme> themes, ICookieProvider cookies, IJSRuntime js)
    {
        _cookies = cookies;
        _js = js;

        // Preserve registration order. The first registered theme is the default
        // when no cookie is present — this is why AddDashboard registers Default
        // first.
        Available = themes.ToArray();
        if (Available.Count == 0)
            throw new InvalidOperationException(
                "No IDashboardTheme registered. AddDashboard() seeds three built-ins; " +
                "verify it ran before resolving IThemeManager.");

        Current = Available[0];
        Mode = ThemeMode.Auto;

        // Eager cookie read during construction — ICookieProvider is request-scoped
        // and CookieMiddleware has already seeded the repo from
        // HttpContext.Request.Cookies by the time DI resolves us inside SSR. This
        // means the very first render already carries the user's persisted choice;
        // HydrateAsync later only re-bridges the request → circuit scope split
        // when running interactively. The result: no flash of the default theme
        // on returning visitors.
        ReadFromCookies();
    }

    public IDashboardTheme Current { get; private set; }
    public ThemeMode Mode { get; private set; }
    public IReadOnlyList<IDashboardTheme> Available { get; }

    public bool IsDark => Mode switch
    {
        ThemeMode.Light => false,
        ThemeMode.Dark  => true,
        _               => _systemPrefersDark, // Auto
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

        // ---- 2. Live system color-scheme query (Auto mode only). ----
        // Only worth a JSInterop round-trip when the user actually has Auto mode
        // selected; Light / Dark resolve without it.
        if (Mode == ThemeMode.Auto)
        {
            try
            {
                var live = await _js.InvokeAsync<bool>(
                    "eval",
                    "window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches");
                if (live != _systemPrefersDark)
                {
                    _systemPrefersDark = live;
                    changed = true;
                }

                // Persist for next SSR — see SystemDarkCookieKey on the field.
                await _cookies.SetAsync(SystemDarkCookieKey, live ? "1" : "0", TimeSpan.FromDays(365));
            }
            catch (InvalidOperationException)
            {
                // Prerender — cookie value (if any) already applied by ReadFromCookies.
            }
        }

        if (changed)
            OnChange?.Invoke();
    }

    /// <summary>
    /// Pulls theme / mode / system-dark from the cookie repository into the
    /// in-memory state. Returns <see langword="true"/> when something actually
    /// changed — callers raise <see cref="OnChange"/> only if it did.
    /// </summary>
    private bool ReadFromCookies()
    {
        var changed = false;

        var themeCookie = _cookies.Get(ThemeCookieKey)?.Value;
        if (!string.IsNullOrWhiteSpace(themeCookie))
        {
            var picked = Available.FirstOrDefault(
                t => string.Equals(t.Id, themeCookie, StringComparison.OrdinalIgnoreCase));
            if (picked is not null && !ReferenceEquals(picked, Current))
            {
                Current = picked;
                changed = true;
            }
        }

        var modeCookie = _cookies.Get(ModeCookieKey)?.Value;
        if (Enum.TryParse<ThemeMode>(modeCookie, ignoreCase: true, out var parsed) && parsed != Mode)
        {
            Mode = parsed;
            changed = true;
        }

        // System dark preference is cached in a cookie set by the inline script in
        // DashboardApp.razor BEFORE Blazor boots — so SSR can render the correct
        // dark/light HTML on every visit after the first. "1" / "0" to keep the
        // cookie one byte; missing cookie falls back to false (light), which is
        // overridden the moment HydrateAsync runs JSInterop in the circuit.
        var systemDark = _cookies.Get(SystemDarkCookieKey)?.Value == "1";
        if (systemDark != _systemPrefersDark)
        {
            _systemPrefersDark = systemDark;
            changed = true;
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
