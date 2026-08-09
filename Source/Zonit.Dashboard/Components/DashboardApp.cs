using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Components;

/// <summary>
/// Root component for a <c>UseDashboard()</c> mount.
/// </summary>
/// <remarks>
/// <para><b>Almost nothing left here, and that is the point.</b> The document shell — the
/// <c>lang</c> attribute, <c>&lt;base href&gt;</c> derived from the request path base, the
/// scoped-CSS bundle, <c>ImportMap</c>, <c>HeadOutlet</c>, the prerender state bridge, the
/// framework script — is <see cref="AppBase"/>'s job now. This file used to hand-write all of it,
/// including a <c>&lt;base href&gt;</c> whose comment ran to eight lines explaining a bug that
/// every other host would have had to rediscover.</para>
///
/// <para>What the dashboard still contributes is declared, not written: stylesheets, scripts and
/// components go on <c>SiteOptions.Document</c> in <c>DashboardSiteOptions.OnConfiguring</c>. The
/// one thing that needs a real override is the router, because the dashboard's "not authorized"
/// panel is MudBlazor markup and does not belong in the framework.</para>
/// </remarks>
public sealed class DashboardApp : AppBase
{
    /// <inheritdoc />
    /// <remarks>
    /// The dashboard keeps its own router for one reason: the 403 panel. <c>WebsiteRoutes</c>
    /// deliberately has no not-found or not-authorized fragment — an unmatched route is a status
    /// code, handled by the branch's status-code re-execution — but an <em>authorization</em>
    /// failure on a matched route is a real page, and this one is drawn with MudBlazor
    /// components the kernel does not reference.
    /// </remarks>
    protected override void BuildRoutes(RenderTreeBuilder builder, IComponentRenderMode? renderMode)
    {
        builder.OpenComponent<Routes>(0);
        if (renderMode is not null)
            builder.AddComponentRenderMode(renderMode);
        builder.CloseComponent();
    }
}
