using Microsoft.Extensions.DependencyInjection;
using Zonit.Dashboard.Extensions;
using Zonit.Dashboard.Extensions.Builtin;

namespace Zonit.Extensions;

/// <summary>
/// Opt-in registration for the dashboard's task-manager UI extensions. Kept
/// separate from <c>AddDashboard()</c> because <c>Zonit.Messaging.Tasks</c> is
/// an optional dependency — hosts that don't run background tasks should not
/// pay the cost of the toolbar widget querying an empty
/// <c>ITaskManager</c> on every render.
/// </summary>
public static class ServiceCollectionTaskManagerExtensions
{
    /// <summary>
    /// Registers the toolbar (badge + count) and drawer (live list) extensions
    /// for the <c>Zonit.Messaging.Tasks.ITaskManager</c> background-task system.
    /// Call after <c>AddDashboard()</c> and after the host's
    /// <c>AddTaskProvider()</c> (or equivalent) so <c>ITaskManager</c>
    /// is resolvable when the components render.
    /// </summary>
    public static IServiceCollection AddDashboardTaskManager(this IServiceCollection services)
    {
        services.AddToolbarExtension<TaskManagerToolbarExtension>();
        services.AddDrawerExtension<TaskManagerDrawerExtension>();
        return services;
    }
}
