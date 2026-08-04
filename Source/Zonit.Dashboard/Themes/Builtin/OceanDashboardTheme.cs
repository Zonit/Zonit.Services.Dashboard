using MudBlazor;

namespace Zonit.Dashboard.Themes.Builtin;

/// <summary>
/// Built-in dashboard theme — cool ocean blue. Ignores tenant brand colors by
/// design: the theme selector exists precisely so users can pick an aesthetic
/// that overrides the tenant default. Palette taken verbatim from the legacy
/// dashboard's <c>OceanDashboardTheme</c>.
/// </summary>
internal sealed class OceanDashboardTheme : IDashboardTheme
{
    public string Id => "ocean";
    public string Name => "Ocean";
    public string Description => "Cool ocean blue.";

    // Cached — palette is static, no need to rebuild per call.
    public MudTheme MudTheme { get; } = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary   = "#0284c7",
            Secondary = "#0891b2",
            Tertiary  = "#0ea5e9",
            Info      = "#38bdf8",
            Success   = "#10b981",
            Warning   = "#f59e0b",
            Error     = "#ef4444",

            Background       = "#f0f9ff",
            Surface          = "#ffffff",
            AppbarBackground = "#ffffff",
            DrawerBackground = "#ffffff",
            TextPrimary      = "#0c4a6e",
            TextSecondary    = "#0369a1",
            TextDisabled     = "rgba(0,0,0,0.38)",
            AppbarText       = "#075985",
            DrawerText       = "#0369a1",
            ActionDefault    = "#0284c7",
            Divider          = "rgba(0,0,0,0.12)",
            HoverOpacity     = 0.06,
            RippleOpacity    = 0.10,
        },
        PaletteDark = new PaletteDark
        {
            Primary   = "#38bdf8",
            Secondary = "#22d3ee",
            Tertiary  = "#7dd3fc",
            Info      = "#67e8f9",
            Success   = "#34d399",
            Warning   = "#fbbf24",
            Error     = "#f87171",

            Background       = "#0c1929",
            Surface          = "#0f2740",
            AppbarBackground = "#0d1f33",
            DrawerBackground = "#0d1f33",
            TextPrimary      = "rgba(255,255,255,0.87)",
            TextSecondary    = "rgba(255,255,255,0.60)",
            TextDisabled     = "rgba(255,255,255,0.38)",
            AppbarText       = "rgba(255,255,255,0.87)",
            DrawerText       = "rgba(255,255,255,0.70)",
            ActionDefault    = "#7dd3fc",
            Divider          = "rgba(255,255,255,0.12)",
            HoverOpacity     = 0.08,
            RippleOpacity    = 0.12,
        },
        // Shared with every other built-in theme: Ocean and Forest differ from Default in
        // palette only, so the type scale and corner radius must not diverge between them.
        Typography = DashboardTypography.Create(),
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
        },
    };
}
