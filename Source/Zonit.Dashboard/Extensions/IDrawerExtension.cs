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

    /// <summary>Drawer width in pixels when open.</summary>
    int Width => 320;

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
