using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Zonit.Dashboard;

/// <summary>
/// Per-request marker identifying which dashboard mount is serving the current
/// request, plus the dashboard-specific extras configured for that mount (layout
/// knobs, extension whitelist, custom snippet). Mirrors
/// <see cref="Zonit.Extensions.Website.ICurrentSite"/> in spirit but only carries
/// dashboard-private state — the website-level <c>ICurrentSite</c> covers everything
/// shared with non-dashboard sites (Directory, Permission, Areas list).
/// </summary>
/// <remarks>
/// <para>Scoped service. The default implementation self-hydrates from
/// <see cref="DashboardMountRegistry"/> (a singleton populated by every
/// <c>UseDashboard</c> call at startup) whenever the dashboard's branch middleware
/// has not had a chance to call <see cref="Set"/> on the current scope — most
/// notably in the SignalR <em>circuit</em> scope owning interactive Blazor
/// components, where <c>branch.Use(...)</c> never fires. Without the fallback the
/// dashboard layout would render correctly during SSR (HTTP scope alive) and then
/// flash to the <see cref="DashboardLayoutOptions"/> defaults the moment the page
/// hydrates — exactly the "Layout briefly correct, then snaps to default widths"
/// bug reported against the multi-mount Documents host.</para>
///
/// <para>The fallback infers the active mount from
/// <c>HttpContext.Request.PathBase</c> when an HTTP context is available
/// and from <see cref="NavigationManager.BaseUri"/> when it is not (interactive
/// circuit scope, hosted services). Both signals match the singleton lookup key
/// produced by <see cref="DashboardMountRegistry.Register"/> — the same path-form
/// <c>ICurrentSite</c> uses against <see cref="Zonit.Extensions.Website.WebsiteMountRegistry"/>.</para>
/// </remarks>
public interface IDashboardCurrentSite
{
    /// <summary>
    /// <see langword="true"/> when either the dashboard branch middleware has
    /// called <see cref="Set"/> or <see cref="DashboardMountRegistry"/> resolved a
    /// snapshot for the current request / circuit. <see langword="false"/> only when
    /// the consumer is reading outside any registered dashboard mount.
    /// </summary>
    bool IsSet { get; }

    /// <summary>Effective layout options for the current mount (never <see langword="null"/>).</summary>
    DashboardLayoutOptions Layout { get; }

    /// <summary>Extension whitelist for the current mount; <see langword="null"/> = no filter.</summary>
    string[]? ExtensionsWhitelist { get; }

    /// <summary>Custom HTML/JS snippet for the current mount; <see langword="null"/> = none.</summary>
    string? CustomSnippet { get; }

    /// <summary>Per-mount theme overrides (never <see langword="null"/>; empty when no overrides set).</summary>
    DashboardThemeOverrides ThemeOverrides { get; }

    /// <summary>
    /// Where this mount's identity screens live and what they post to. Never <see langword="null"/>;
    /// an unconfigured mount reports no sign-in endpoint, which the built-in login screen renders
    /// as an explicit "not configured" panel rather than a form that cannot work.
    /// </summary>
    DashboardIdentityOptions Identity { get; }

    /// <summary>
    /// Called once by the dashboard branch middleware. Subsequent calls overwrite
    /// and pin the explicit values, so the singleton fallback is no longer
    /// consulted on this scope.
    /// </summary>
    void Set(DashboardLayoutOptions layout, string[]? extensionsWhitelist, string? customSnippet, DashboardThemeOverrides themeOverrides, DashboardIdentityOptions identity);
}

internal sealed class DashboardCurrentSite : IDashboardCurrentSite
{
    private static readonly DashboardLayoutOptions Defaults = new();
    private static readonly DashboardThemeOverrides EmptyTheme = new();
    private static readonly DashboardIdentityOptions DefaultIdentity = new();

    private readonly DashboardMountRegistry _mounts;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IServiceProvider _services;

    // Explicit middleware-driven state. When _explicit is true these win;
    // otherwise every accessor falls through to the resolved registry snapshot.
    private bool _explicit;
    private DashboardLayoutOptions _layout = Defaults;
    private string[]? _extensionsWhitelist;
    private string? _customSnippet;
    private DashboardThemeOverrides _themeOverrides = EmptyTheme;
    private DashboardIdentityOptions _identity = DefaultIdentity;

    // Cached singleton lookup. _resolveAttempted suppresses repeat NavigationManager
    // probes inside the same scope (BaseUri throws if accessed before the circuit
    // initialised it; we want to fail open exactly once per scope).
    private DashboardMountRegistry.MountSnapshot? _resolved;
    private bool _resolveAttempted;

    public DashboardCurrentSite(
        DashboardMountRegistry mounts,
        IHttpContextAccessor httpContext,
        IServiceProvider services)
    {
        _mounts = mounts;
        _httpContext = httpContext;
        _services = services;
    }

    public bool IsSet => _explicit || ResolveSnapshot() is not null;

    public DashboardLayoutOptions Layout => _explicit
        ? _layout
        : ResolveSnapshot()?.Layout ?? Defaults;

    public string[]? ExtensionsWhitelist => _explicit
        ? _extensionsWhitelist
        : ResolveSnapshot()?.ExtensionsWhitelist;

    public string? CustomSnippet => _explicit
        ? _customSnippet
        : ResolveSnapshot()?.CustomSnippet;

    public DashboardThemeOverrides ThemeOverrides => _explicit
        ? _themeOverrides
        : ResolveSnapshot()?.ThemeOverrides ?? EmptyTheme;

    public DashboardIdentityOptions Identity => _explicit
        ? _identity
        : ResolveSnapshot()?.Identity ?? DefaultIdentity;

    public void Set(DashboardLayoutOptions layout, string[]? extensionsWhitelist, string? customSnippet, DashboardThemeOverrides themeOverrides, DashboardIdentityOptions identity)
    {
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(themeOverrides);
        ArgumentNullException.ThrowIfNull(identity);
        _layout = layout;
        _extensionsWhitelist = extensionsWhitelist;
        _customSnippet = customSnippet;
        _themeOverrides = themeOverrides;
        _identity = identity;
        _explicit = true;
    }

    /// <summary>
    /// Resolve the owning mount snapshot lazily. The lookup runs at most once per
    /// scope; subsequent accessor reads cost a single cached-field read.
    /// </summary>
    private DashboardMountRegistry.MountSnapshot? ResolveSnapshot()
    {
        if (_resolveAttempted) return _resolved;
        _resolveAttempted = true;

        var path = ResolveActivePath();
        if (path is null) return null;

        _resolved = _mounts.SnapshotFor(path);
        return _resolved;
    }

    /// <summary>
    /// Mirror of <c>CurrentSite.ResolveActivePath</c> — HttpContext.Request.PathBase
    /// first (set by <c>UsePathBase</c> inside the branch), Request.Path next
    /// (global pre-branch middleware), <see cref="NavigationManager.BaseUri"/>
    /// last (interactive circuit scope with no HTTP request).
    /// </summary>
    private string? ResolveActivePath()
    {
        var http = _httpContext.HttpContext;
        if (http is not null)
        {
            if (http.Request.PathBase.HasValue)
                return http.Request.PathBase.Value;
            if (http.Request.Path.HasValue)
                return http.Request.Path.Value;
        }

        var nav = _services.GetService(typeof(NavigationManager)) as NavigationManager;
        if (nav is null) return null;

        try
        {
            return new Uri(nav.BaseUri).AbsolutePath;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
