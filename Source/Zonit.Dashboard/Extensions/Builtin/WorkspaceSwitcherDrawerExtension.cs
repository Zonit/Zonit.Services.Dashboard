using MudBlazor;
using Zonit.Dashboard.Components.Drawers;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Built-in drawer extension for the workspace (organization) switcher. Renders
/// <see cref="WorkspacePanel"/> in the right-side drawer; the toolbar toggle is
/// the Business icon. Hosts can suppress this extension via
/// <c>DashboardSiteOptions.ExtensionsWhitelist</c> when they don't use the
/// Organizations module.
/// </summary>
public sealed class WorkspaceSwitcherDrawerExtension : DrawerExtensionBase<WorkspacePanel>
{
    public override string Id => "zonit.dashboard.workspace";
    public override string Name => "Workspace";
    public override int Order => 200;
    public override string? Icon => Icons.Material.Filled.Business;
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 320;
}
