using Zonit.Dashboard.Components.Drawers;
using Zonit.Dashboard.Extensions;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Built-in drawer extension that shows a theme picker (built-in themes + Light/Dark/Auto
/// mode) in the right-anchor drawer. Registered automatically by <c>AddDashboard()</c>
/// so a fresh mount always has at least one extension proving the system works end-to-end.
/// </summary>
/// <remarks>
/// Hosts that don't want a theme selector visible can disable it via the per-mount
/// <c>DashboardSiteOptions.ExtensionsWhitelist</c> (omit
/// <c>"zonit.dashboard.theme"</c> from the list).
/// </remarks>
internal sealed class ThemeSelectorDrawerExtension : DrawerExtensionBase<ThemeSelectorPanel>
{
    public override string Id => "zonit.dashboard.theme";
    public override string Name => "Theme";
    public override int Order => 1000; // late — most plug-ins go before
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 320;
    public override string? Icon => MudBlazor.Icons.Material.Filled.Palette;
}
