using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.Dashboard.Areas.Dashboard.Components;
using Zonit.Services.Dashboard.Components;

namespace Zonit.Services.Dashboard.DrawerExtensions;

/// <summary>
/// Culture drawer extension for language selection.
/// AOT-safe implementation using DrawerExtensionBase.
/// </summary>
public sealed class CultureDrawerExtension : DrawerExtensionBase<CultureDrawer>
{
    /// <inheritdoc />
    public override string Id => DashboardExtensions.Cultures;

    /// <inheritdoc />
    public override string Name => "Cultures";

    /// <inheritdoc />
    public override int Order => 100;

    /// <inheritdoc />
    public override DrawerAnchor Anchor => DrawerAnchor.End;

    /// <inheritdoc />
    public override int Width => 300;
}

/// <summary>
/// Task manager drawer extension.
/// AOT-safe implementation using DrawerExtensionBase.
/// </summary>
public sealed class TaskDrawerExtension : DrawerExtensionBase<TaskDrawer>
{
    /// <inheritdoc />
    public override string Id => DashboardExtensions.Tasks;

    /// <inheritdoc />
    public override string Name => "Task Manager";

    /// <inheritdoc />
    public override int Order => 50;

    /// <inheritdoc />
    public override DrawerAnchor Anchor => DrawerAnchor.End;

    /// <inheritdoc />
    public override int Width => 420;
}
