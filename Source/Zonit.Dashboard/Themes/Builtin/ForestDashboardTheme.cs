using MudBlazor;

namespace Zonit.Dashboard.Themes.Builtin;

/// <summary>
/// Built-in dashboard theme — nature-inspired green. Ignores tenant brand colors
/// by design (see <see cref="OceanDashboardTheme"/>). Palette taken verbatim from
/// the legacy dashboard's <c>ForestDashboardTheme</c>.
/// </summary>
internal sealed class ForestDashboardTheme : IDashboardTheme
{
    public string Id => "forest";
    public string Name => "Forest";
    public string Description => "Nature-inspired green.";

    public MudTheme MudTheme { get; } = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary   = "#059669",
            Secondary = "#10b981",
            Tertiary  = "#34d399",
            Info      = "#3b82f6",
            Success   = "#22c55e",
            Warning   = "#f59e0b",
            Error     = "#ef4444",

            Background       = "#f0fdf4",
            Surface          = "#ffffff",
            AppbarBackground = "#ffffff",
            DrawerBackground = "#ffffff",
            TextPrimary      = "#14532d",
            TextSecondary    = "#166534",
            TextDisabled     = "rgba(0,0,0,0.38)",
            AppbarText       = "#15803d",
            DrawerText       = "#166534",
            ActionDefault    = "#059669",
            Divider          = "rgba(0,0,0,0.12)",
            HoverOpacity     = 0.06,
            RippleOpacity    = 0.10,
        },
        PaletteDark = new PaletteDark
        {
            Primary   = "#34d399",
            Secondary = "#4ade80",
            Tertiary  = "#6ee7b7",
            Info      = "#60a5fa",
            Success   = "#4ade80",
            Warning   = "#fbbf24",
            Error     = "#f87171",

            Background       = "#0a1f13",
            Surface          = "#0f2d1a",
            AppbarBackground = "#0c2516",
            DrawerBackground = "#0c2516",
            TextPrimary      = "rgba(255,255,255,0.87)",
            TextSecondary    = "rgba(255,255,255,0.60)",
            TextDisabled     = "rgba(255,255,255,0.38)",
            AppbarText       = "rgba(255,255,255,0.87)",
            DrawerText       = "rgba(255,255,255,0.70)",
            ActionDefault    = "#6ee7b7",
            Divider          = "rgba(255,255,255,0.12)",
            HoverOpacity     = 0.08,
            RippleOpacity    = 0.12,
        },
    };
}
