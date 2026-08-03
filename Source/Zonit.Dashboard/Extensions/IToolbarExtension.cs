using Microsoft.AspNetCore.Components;

namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Dashboard extension that contributes an appbar action slot — buttons,
/// drop-down menus, badges. Multiple extensions can populate the same
/// <see cref="Position"/>; they render in <see cref="IDashboardExtension.Order"/>
/// order.
/// </summary>
/// <remarks>
/// <para>Implementations should derive from <see cref="ToolbarExtensionBase{TComponent}"/>
/// so the AOT-safe <see cref="CreateToolbarContent"/> factory is supplied for them.</para>
/// </remarks>
public interface IToolbarExtension : IDashboardExtension
{
    /// <summary>Slot within the appbar (start / center / end).</summary>
    ToolbarPosition Position => ToolbarPosition.End;

    /// <summary>
    /// Builds the <see cref="RenderFragment"/> rendered inside the appbar's
    /// <see cref="Position"/> slot. AOT-safe.
    /// </summary>
    RenderFragment CreateToolbarContent();
}
