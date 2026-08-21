using Microsoft.AspNetCore.Components;

namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Dashboard extension that contributes a side-drawer slot — typically a notification
/// feed, task list, or quick-action panel hung off the dashboard's appbar.
/// </summary>
/// <remarks>
/// <para>Implementations almost always derive from
/// <see cref="DrawerExtensionBase{TComponent}"/> rather than implementing this
/// interface directly — the base class supplies the AOT-safe
/// <see cref="CreateDrawerContent"/> factory and the trimmer-friendly type binding.</para>
///
/// <para>The dashboard's <c>DashboardMainLayout</c> consumes drawer extensions via
/// <see cref="IExtensionRegistry.GetDrawerExtensions"/>, opens / closes them
/// through <see cref="IExtensionDrawerStates"/>, and renders the content fragment
/// inside a single shared <c>MudDrawer</c> per anchor side.</para>
/// </remarks>
public interface IDrawerExtension : IDashboardExtension
{
    /// <summary>Which side the drawer docks to.</summary>
    DrawerAnchor Anchor => DrawerAnchor.End;

    /// <summary>
    /// Drawer width in pixels when open, or <c>0</c> to take the mount's
    /// <see cref="DashboardLayoutOptions.RightDrawerWidth"/>.
    /// </summary>
    /// <remarks>
    /// The default is 0, not a width. It used to be a hard 320, which meant
    /// <c>RightDrawerWidth</c> — a documented, settable per-mount option — was read by nothing at
    /// all: a host that set it got no effect and no warning. Extensions that genuinely need a
    /// specific width still state one and it still wins; everything else now follows the mount.
    /// <c>RightDrawerWidth</c> defaults to the same 320 these panels were already getting, so
    /// nothing moves for a host that has not set it.
    /// </remarks>
    int Width => 0;

    /// <summary>Optional Material icon shown on the toolbar toggle button (e.g. <c>Icons.Material.Filled.Notifications</c>).</summary>
    string? Icon => null;

    /// <summary>
    /// Builds the <see cref="RenderFragment"/> that fills the drawer body when this
    /// extension is the active drawer. AOT-safe — no reflection;
    /// <see cref="DrawerExtensionBase{TComponent}"/> wires
    /// <c>builder.OpenComponent&lt;TComponent&gt;()</c> directly.
    /// </summary>
    RenderFragment CreateDrawerContent();
}
