namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Shared contract for any extension that contributes UI to the dashboard chrome.
/// Specialised by <see cref="IDrawerExtension"/> (side-panel slots) and
/// <see cref="IToolbarExtension"/> (appbar action slots).
/// </summary>
/// <remarks>
/// <para><b>What's not here</b> (vs the legacy <c>IDashboardExtension</c>):</para>
/// <list type="bullet">
///   <item><c>INavigationExtension</c> — gone. Navigation is contributed by every
///         area through <see cref="Zonit.Extensions.Website.IWebsiteArea.Navigation"/>
///         (<c>NavGroup</c> / <c>NavItem</c>); the dashboard's
///         <c>INavigationProvider</c> aggregates them at runtime. The legacy parallel
///         registry was redundant.</item>
///   <item><c>ISettingsExtension</c> — gone. Tenant-level settings live in
///         <c>Tenant.Settings</c> (<see cref="Zonit.Extensions.Tenants.Settings.Setting{T}"/>
///         derivatives); per-user settings are regular pages routed under the
///         area's nav. A separate "settings panel slot" added no new affordance.</item>
/// </list>
///
/// <para><b>Lifetimes</b>: extensions register as singletons by default
/// (<c>services.AddDrawerExtension&lt;T&gt;()</c>). Use the factory overload if
/// the extension needs to capture scoped state at resolution time.</para>
/// </remarks>
public interface IDashboardExtension
{
    /// <summary>Stable identifier — used for <c>ExtensionsWhitelist</c>, cookies, and the drawer-state lookup.</summary>
    string Id { get; }

    /// <summary>Display name shown in tooltips / settings UIs.</summary>
    string Name { get; }

    /// <summary>Sort order (ascending) within a slot.</summary>
    int Order => 0;

    /// <summary>
    /// Per-instance enable switch — checked alongside the per-mount
    /// <c>DashboardSiteOptions.ExtensionsWhitelist</c>. Use this for run-time toggles
    /// (e.g. feature flags); use the whitelist for compile-time / configuration toggles.
    /// </summary>
    bool IsEnabled => true;
}
