using MudBlazor;
using Zonit.Dashboard.Components.Drawers;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Drawer slot rendering the live list of background tasks
/// (<see cref="TaskManagerPanel"/>). Toggle is owned by
/// <see cref="TaskManagerToolbarExtension"/>; we still register an Icon here
/// because the dashboard layout also auto-wires a generic toggle button for
/// every drawer extension that has one — and the explicit toolbar slot can
/// share the same icon when the host disables the manual toolbar entry.
/// </summary>
public sealed class TaskManagerDrawerExtension : DrawerExtensionBase<TaskManagerPanel>
{
    public override string Id => "zonit.dashboard.tasks";
    public override string Name => "Background Tasks";
    public override int Order => 500;
    public override string? Icon => Icons.Material.Filled.Task;
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 420;
}
