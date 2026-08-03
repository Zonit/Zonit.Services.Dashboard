using MudBlazor;
using Zonit.Dashboard.Components.Drawers;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Built-in drawer extension for the project (catalog) switcher. Twin of
/// <see cref="WorkspaceSwitcherDrawerExtension"/> — same anchor, narrower scope
/// (one project from the active workspace). Hosts that don't use the Projects
/// module should disable this via <c>DashboardSiteOptions.ExtensionsWhitelist</c>.
/// </summary>
public sealed class ProjectSwitcherDrawerExtension : DrawerExtensionBase<ProjectPanel>
{
    public override string Id => "zonit.dashboard.project";
    public override string Name => "Project";
    public override int Order => 210;
    public override string? Icon => Icons.Material.Filled.Folder;
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 320;
}
