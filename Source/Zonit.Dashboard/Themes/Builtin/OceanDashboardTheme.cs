using MudBlazor;

namespace Zonit.Dashboard.Themes.Builtin;

/// <summary>
/// Built-in dashboard theme — cool ocean blue. Ignores tenant brand colors by
/// design: the theme selector exists precisely so users can pick an aesthetic
/// that overrides the tenant default. Palette taken verbatim from the legacy
/// dashboard's <c>OceanDashboardTheme</c>.
/// </summary>
/// <remarks>
/// <para>Brand slots go through <see cref="ThemeColors.EnsureContrast"/> against the surface
/// they are painted on, exactly as the Default theme does. These palettes were hand-picked and
/// several of the picks did not survive measurement: Forest's light Tertiary #34d399 scored
/// 1.7:1 on white and its Secondary 2.3:1, Ocean's light Tertiary 2.6:1 — all of them well under
/// AA for the colour that paints links and button labels. Running them through the same pass
/// keeps the aesthetic (hue and saturation are untouched) and removes the possibility of a
/// future hand-picked value quietly failing again.</para>
///
/// <para>Status colours are per-mode literals for the same reason as in the Default theme: the
/// previous light set (#f59e0b at 1.9:1, #10b981 at 2.5:1) was chosen to look bright rather than
/// to be read.</para>
/// </remarks>
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
            Primary   = ThemeColors.EnsureContrast("#0284c7", "#ffffff", ThemeColors.AaText),
            Secondary = ThemeColors.EnsureContrast("#0891b2", "#ffffff", ThemeColors.AaText),
            Tertiary  = ThemeColors.EnsureContrast("#0ea5e9", "#ffffff", ThemeColors.AaText),
            Info      = "#0369a1",
            Success   = "#047857",
            Warning   = "#b45309",
            Error     = "#c2263a",

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
            Divider          = "rgba(15,23,42,0.18)",
            HoverOpacity     = 0.06,
            RippleOpacity    = 0.10,
        },
        PaletteDark = new PaletteDark
        {
            Primary   = ThemeColors.EnsureContrast("#38bdf8", "#123049", ThemeColors.AaText),
            Secondary = ThemeColors.EnsureContrast("#22d3ee", "#123049", ThemeColors.AaText),
            Tertiary  = ThemeColors.EnsureContrast("#7dd3fc", "#123049", ThemeColors.AaText),
            Info      = "#67e8f9",
            Success   = "#34d399",
            Warning   = "#fbbf24",
            Error     = "#f87171",

            Background       = "#08131f",
            Surface          = "#123049",
            AppbarBackground = "#0c2136",
            DrawerBackground = "#0c2136",
            TextPrimary      = "rgba(255,255,255,0.92)",
            TextSecondary    = "rgba(255,255,255,0.66)",
            TextDisabled     = "rgba(255,255,255,0.38)",
            AppbarText       = "rgba(255,255,255,0.92)",
            DrawerText       = "rgba(255,255,255,0.74)",
            ActionDefault    = "#7dd3fc",
            Divider          = "rgba(255,255,255,0.16)",
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
