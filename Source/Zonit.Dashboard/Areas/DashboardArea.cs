using Microsoft.Extensions.DependencyInjection;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard;

/// <summary>
/// The dashboard itself as a Website <see cref="IWebsiteArea"/>. Registered
/// automatically by <c>AddDashboard()</c> — consumers don't call
/// <c>o.AddArea&lt;DashboardArea&gt;()</c> themselves.
/// </summary>
/// <remarks>
/// <para>Owns no top-level navigation entries of its own. The whole point of the
/// rewrite is that <em>other</em> areas (e.g. <c>OrganizationsArea</c>,
/// <c>TenantsArea</c>, plug-in areas) supply <c>NavGroup</c> / <c>NavItem</c>
/// entries through <see cref="IWebsiteArea.Navigation"/> and the dashboard's
/// <c>MainLayout</c> aggregates them via <c>INavigationProvider</c>. No more
/// dashboard-private <c>INavigationManager</c>.</para>
///
/// <para>This area's <see cref="IWebsiteServices.ConfigureServices"/> is intentionally
/// empty — dashboard services are wired by <c>AddDashboard()</c> directly so they
/// are available even before this area's <c>ConfigureServices</c> runs (which matters
/// for the <c>"Dashboard.Main"</c> layout seed and similar registrations).</para>
/// </remarks>
public sealed class DashboardArea : IWebsiteArea, IWebsiteServices
{
    /// <summary>Canonical area key; stable across versions.</summary>
    public string Key => "Zonit.Dashboard";

    /// <summary>No own navigation — see remarks on the class.</summary>
    public IReadOnlyList<NavGroup> Navigation { get; } = Array.Empty<NavGroup>();

    /// <summary>
    /// Intentionally a no-op. All dashboard DI happens in <c>AddDashboard()</c> so
    /// it can read the <c>DashboardOptions</c> instance the consumer just
    /// configured. Keeping this method present (rather than not implementing
    /// <see cref="IWebsiteServices"/>) makes the area's intent explicit: yes, it
    /// participates in the IWebsiteServices contract, just with nothing to add here.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
    }
}
