using System.Reflection;
using Zonit.Extensions;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard;

/// <summary>
/// Singleton map of dashboard mount path → route assemblies of areas mounted there.
/// Populated by <c>UseDashboard()</c> at startup, consumed by <c>Routes.razor</c>'s
/// <c>&lt;Router AdditionalAssemblies&gt;</c>.
/// </summary>
/// <remarks>
/// <para><b>Why a singleton?</b> <see cref="ICurrentSite.Areas"/> and
/// <see cref="IDashboardCurrentSite"/> are both <em>scoped</em> services populated by
/// per-Site branch middleware (<c>branch.Use(...)</c> in <c>UseWebsite</c> /
/// <c>UseDashboard</c>). That middleware only runs for HTTP requests; the SignalR
/// circuit lifecycle that owns interactive Blazor components does NOT pass through
/// it, so a freshly-created circuit-scope <c>ICurrentSite</c> stays in its unset
/// default (<c>IsSet=false</c>, <c>Areas=Array.Empty</c>).</para>
///
/// <para>Reading area assemblies from a scoped service inside
/// <c>&lt;Routes @rendermode="InteractiveServer"&gt;</c> therefore produces an empty
/// list as soon as the page hydrates — the SSR pass renders fine (HTTP scope is
/// alive), but the very first client-side navigation event yields
/// <c>"The following routes are ambiguous"</c> (when falling back to the global
/// <see cref="WebsiteAreaRegistry"/>, which holds <em>every</em> registered area
/// including <c>@page "/"</c> from the host site) or <c>"Not Found"</c> (when
/// falling back to <c>ICurrentSite.Areas</c>, which is empty).</para>
///
/// <para>A singleton keyed by mount path side-steps both: it's populated once at
/// build time with exactly the areas mounted on this dashboard (no leakage from
/// the host's root Site), and it survives the HTTP-scope → circuit-scope transition
/// because it isn't scoped at all.</para>
///
/// <para><b>Lookup key.</b> Mount paths are normalised to their
/// <c>SiteOptions.NormalizedPathBase</c> form (empty string for root mount,
/// otherwise rooted segment without trailing slash — e.g. <c>"/admin"</c>). The
/// Routes component resolves the active key from <c>NavigationManager.BaseUri</c>
/// (also available in the circuit scope, unlike middleware-populated state).</para>
/// </remarks>
public sealed class DashboardMountRegistry
{
    private readonly Dictionary<string, MountSnapshot> _byMount =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers the per-mount snapshot for a single <c>UseDashboard()</c> call.
    /// Captures both the route assemblies consumed by <c>Routes.razor</c>'s
    /// <c>&lt;Router AdditionalAssemblies&gt;</c> and the dashboard-specific UI
    /// state (<see cref="DashboardLayoutOptions"/>, extension whitelist, custom
    /// snippet) read by <see cref="IDashboardCurrentSite"/>. Overwrites any
    /// previous registration for the same mount (last call wins — consistent with
    /// multi-mount overrides).
    /// </summary>
    public void Register(
        UrlPath directory,
        IEnumerable<IWebsiteArea> areas,
        DashboardLayoutOptions layout,
        string[]? extensionsWhitelist,
        string? customSnippet,
        DashboardThemeOverrides? themeOverrides = null)
    {
        ArgumentNullException.ThrowIfNull(areas);
        ArgumentNullException.ThrowIfNull(layout);

        var key = Normalize(directory);
        var assemblies = areas
            .Select(a => a.ComponentsAssembly)
            .Where(a => a is not null)
            .Distinct()
            .ToArray()!;

        _byMount[key] = new MountSnapshot(
            Assemblies: assemblies,
            Layout: layout,
            ExtensionsWhitelist: extensionsWhitelist,
            CustomSnippet: customSnippet,
            ThemeOverrides: themeOverrides ?? new DashboardThemeOverrides());
    }

    /// <summary>
    /// Looks up route assemblies for the dashboard mount serving the given URL path.
    /// Returns an empty list when no dashboard is mounted at that prefix (covers the
    /// degenerate case where Routes.razor is rendered outside any UseDashboard branch,
    /// e.g. a host accidentally mounting DashboardApp through plain UseWebsite&lt;&gt;).
    /// </summary>
    /// <param name="absolutePath">
    /// Absolute URL path — typically <c>new Uri(NavigationManager.BaseUri).AbsolutePath</c>.
    /// Trailing slash is tolerated; the path is matched against the registered mount
    /// roots by longest-prefix wins.
    /// </param>
    public IReadOnlyList<Assembly> ForMount(string absolutePath)
        => SnapshotFor(absolutePath)?.Assemblies ?? Array.Empty<Assembly>();

    /// <summary>
    /// Resolves the full per-mount snapshot (assemblies + layout + whitelist +
    /// snippet) for the dashboard serving <paramref name="absolutePath"/>. Returns
    /// <see langword="null"/> when no dashboard is mounted at that prefix — callers
    /// fall back to the defaults baked into <see cref="DashboardCurrentSite"/>.
    /// </summary>
    public MountSnapshot? SnapshotFor(string? absolutePath)
    {
        var path = absolutePath?.TrimEnd('/') ?? string.Empty;

        // Direct hit on the exact normalised key first (fast path for the common
        // case where BaseUri == mount root).
        if (_byMount.TryGetValue(path, out var direct))
            return direct;

        // Longest-prefix-match fallback. Necessary when Routes.razor is rendered on
        // a sub-route (BaseUri still equals the mount root for Blazor, but defensive
        // anyway — and it keeps the API honest if a host ever passes a deeper path).
        MountSnapshot? best = null;
        var bestKeyLength = -1;
        foreach (var (key, snapshot) in _byMount)
        {
            if (key.Length <= bestKeyLength) continue;
            if (key.Length == 0 ||
                path.Equals(key, StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith(key + "/", StringComparison.OrdinalIgnoreCase))
            {
                best = snapshot;
                bestKeyLength = key.Length;
            }
        }

        return best;
    }

    /// <summary>Matches <c>SiteOptions.NormalizedPathBase</c> exactly.</summary>
    private static string Normalize(UrlPath directory)
    {
        if (!directory.HasValue) return string.Empty;
        var v = directory.Value.TrimEnd('/');
        if (v.Length == 0) return string.Empty;
        return v.StartsWith('/') ? v : "/" + v;
    }

    /// <summary>
    /// Immutable per-mount snapshot consumed by <see cref="IDashboardCurrentSite"/>
    /// (Layout / Whitelist / Snippet) and <c>Routes.razor</c>
    /// (<see cref="Assemblies"/>) when the middleware-populated request scope is
    /// unavailable (interactive Blazor circuit, hosted services, anywhere outside
    /// a <c>UseDashboard</c> branch).
    /// </summary>
    public sealed record MountSnapshot(
        IReadOnlyList<Assembly> Assemblies,
        DashboardLayoutOptions Layout,
        string[]? ExtensionsWhitelist,
        string? CustomSnippet,
        DashboardThemeOverrides ThemeOverrides);
}
