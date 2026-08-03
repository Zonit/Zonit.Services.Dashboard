using MudBlazor;
using Zonit.Dashboard.Components.Drawers;

namespace Zonit.Dashboard.Extensions.Builtin;

/// <summary>
/// Built-in drawer extension that hosts the dashboard's culture / language
/// switcher (<see cref="CulturePanel"/>). Hooks the right-side drawer with a
/// language icon toggle in the appbar; the panel itself reads supported cultures
/// from <c>ICultureState</c> and writes selections via <c>ICultureManager</c>.
/// </summary>
/// <remarks>
/// <para>Stable Id <c>"zonit.dashboard.culture"</c> — the panel uses the same Id
/// to drive <see cref="IExtensionDrawerStates"/> close transitions, so changing
/// it here would break the in-panel close button. Keep them in sync.</para>
/// </remarks>
public sealed class CultureSwitcherDrawerExtension : DrawerExtensionBase<CulturePanel>
{
    public override string Id => "zonit.dashboard.culture";
    public override string Name => "Language";
    public override int Order => 100;
    public override string? Icon => Icons.Material.Filled.Language;
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 320;
}
