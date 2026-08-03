using Microsoft.AspNetCore.Builder;
using Zonit.Dashboard;
using Zonit.Dashboard.Components;

namespace Zonit.Extensions;

/// <summary>
/// Middleware-time entry point for <c>Zonit.Dashboard</c>. Pairs with the services-time
/// <c>builder.Services.AddDashboard()</c>.
/// </summary>
/// <remarks>
/// <para>Signature is intentionally <em>1:1 with <see cref="WebsiteServiceCollectionExtensions.UseWebsite{TApp, TSiteOptions}"/></em>:
/// take a <see cref="UrlPath"/> <c>directory</c> + a configuration callback. The only
/// difference is the missing generics — the dashboard bundles <see cref="DashboardApp"/>
/// as the Razor root component and <see cref="DashboardSiteOptions"/> as the per-mount
/// configuration type, so consumers don't have to repeat them.</para>
///
/// <para>The whole specialised pipeline (implicit <see cref="DashboardArea"/>, mount
/// registration into <see cref="DashboardMountRegistry"/>, explicit <c>MapBlazorHub</c>
/// for non-root mounts, per-mount <see cref="IDashboardCurrentSite"/> hydration) lives
/// on <see cref="DashboardSiteOptions"/> as <c>OnConfiguring</c> / <c>OnConfigured</c>
/// template-method overrides — this extension is therefore a one-liner forwarding to
/// the framework's generic inheritance hook.</para>
///
/// <para>Multi-mount safe — call <c>UseDashboard()</c> with different
/// <c>SiteOptions.Directory</c> values to surface the dashboard under two paths
/// with different policies (e.g. <c>/admin</c> for back-office, <c>/operator</c> for a
/// read-only operator view).</para>
/// </remarks>
public static class AppMiddlewareDashboardExtensions
{
    /// <summary>
    /// Mounts the dashboard at the given <paramref name="directory"/> with the
    /// supplied configuration. Mirrors
    /// <see cref="WebsiteServiceCollectionExtensions.UseWebsite{TApp, TSiteOptions}"/>
    /// minus the generics — <see cref="DashboardApp"/> is the Razor root component
    /// and <see cref="DashboardSiteOptions"/> is the per-mount config type.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/>.</param>
    /// <param name="directory">URL prefix; <see cref="UrlPath.Empty"/> = root mount.</param>
    /// <param name="configure">Per-mount configuration callback (Permission, Areas,
    /// Layout knobs, hooks, etc.). <see langword="null"/> mounts with framework defaults
    /// and only the implicit <see cref="DashboardArea"/>.</param>
    public static WebApplication UseDashboard(
        this WebApplication app,
        UrlPath directory,
        Action<DashboardSiteOptions>? configure = null)
        => app.UseWebsite<DashboardApp, DashboardSiteOptions>(directory, configure);
}
