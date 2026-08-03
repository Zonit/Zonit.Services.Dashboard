using MudBlazor;
using Zonit.Dashboard.Components.Toolbar;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Toolbar slot showing the live count of active background tasks
/// (<see cref="TaskManagerToolbar"/>). Pairs with
/// <see cref="TaskManagerDrawerExtension"/> — the toolbar button toggles the
/// drawer; both share the same Id namespace prefix so a host can whitelist /
/// blacklist them as a unit.
/// </summary>
/// <remarks>
/// <para>Opt-in: registered only by <c>services.AddDashboardTaskManager()</c>,
/// not by <c>AddDashboard()</c>, because it requires <c>ITaskManager</c>
/// from <c>Zonit.Messaging.Tasks</c> which not every host wires up.</para>
/// </remarks>
public sealed class TaskManagerToolbarExtension : ToolbarExtensionBase<TaskManagerToolbar>
{
    public override string Id => "zonit.dashboard.tasks.toolbar";
    public override string Name => "Tasks";
    public override int Order => 500;
    public override ToolbarPosition Position => ToolbarPosition.End;
}
