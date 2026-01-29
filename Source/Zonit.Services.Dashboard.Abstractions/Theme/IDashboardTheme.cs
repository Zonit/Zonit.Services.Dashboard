namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Defines a theme for the dashboard.
/// Themes can be registered and selected at runtime.
/// </summary>
public interface IDashboardTheme
{
    /// <summary>
    /// Gets the unique identifier for this theme.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display name of this theme.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of this theme.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets whether this theme supports dark mode.
    /// </summary>
    bool SupportsDarkMode { get; }

    /// <summary>
    /// Gets the theme palette for light mode.
    /// </summary>
    ThemePalette LightPalette { get; }

    /// <summary>
    /// Gets the theme palette for dark mode.
    /// Returns light palette if dark mode is not supported.
    /// </summary>
    ThemePalette DarkPalette { get; }

    /// <summary>
    /// Gets the typography settings for this theme.
    /// </summary>
    ThemeTypography Typography { get; }

    /// <summary>
    /// Gets the layout properties for this theme.
    /// </summary>
    ThemeLayout Layout { get; }
}

/// <summary>
/// Color palette for a theme.
/// </summary>
public record ThemePalette
{
    /// <summary>Primary brand color.</summary>
    public string Primary { get; init; } = "#7764e5";

    /// <summary>Secondary brand color.</summary>
    public string Secondary { get; init; } = "#38bdf8";

    /// <summary>Tertiary/accent color.</summary>
    public string Tertiary { get; init; } = "#9333ea";

    /// <summary>Information color.</summary>
    public string Info { get; init; } = "#3299ff";

    /// <summary>Success color.</summary>
    public string Success { get; init; } = "#0bba83";

    /// <summary>Warning color.</summary>
    public string Warning { get; init; } = "#ffa800";

    /// <summary>Error color.</summary>
    public string Error { get; init; } = "#f64e62";

    /// <summary>Background color.</summary>
    public string Background { get; init; } = "#f8f9fc";

    /// <summary>Surface color (cards, dialogs).</summary>
    public string Surface { get; init; } = "#ffffff";

    /// <summary>Appbar background color.</summary>
    public string AppbarBackground { get; init; } = "#ffffff";

    /// <summary>Drawer background color.</summary>
    public string DrawerBackground { get; init; } = "#ffffff";

    /// <summary>Primary text color.</summary>
    public string TextPrimary { get; init; } = "#1f2937";

    /// <summary>Secondary text color.</summary>
    public string TextSecondary { get; init; } = "#4b5563";

    /// <summary>Disabled text color.</summary>
    public string TextDisabled { get; init; } = "rgba(0,0,0,0.38)";

    /// <summary>Appbar text color.</summary>
    public string AppbarText { get; init; } = "#424c5f";

    /// <summary>Drawer text color.</summary>
    public string DrawerText { get; init; } = "#4b5563";

    /// <summary>Default action color.</summary>
    public string ActionDefault { get; init; } = "#64748b";

    /// <summary>Divider color.</summary>
    public string Divider { get; init; } = "rgba(0,0,0,0.12)";

    /// <summary>Hover opacity (0.0 - 1.0).</summary>
    public double HoverOpacity { get; init; } = 0.06;

    /// <summary>Ripple opacity (0.0 - 1.0).</summary>
    public double RippleOpacity { get; init; } = 0.10;
}

/// <summary>
/// Typography settings for a theme.
/// </summary>
public record ThemeTypography
{
    /// <summary>Default font family.</summary>
    public string[] FontFamily { get; init; } = ["Inter", "Roboto", "system-ui", "sans-serif"];

    /// <summary>Default font size.</summary>
    public string FontSize { get; init; } = "1rem";

    /// <summary>Default line height.</summary>
    public string LineHeight { get; init; } = "1.6";

    /// <summary>Default font weight.</summary>
    public string FontWeight { get; init; } = "400";

    /// <summary>Heading font weight.</summary>
    public string HeadingFontWeight { get; init; } = "600";

    /// <summary>Button font weight.</summary>
    public string ButtonFontWeight { get; init; } = "500";
}

/// <summary>
/// Layout properties for a theme.
/// </summary>
public record ThemeLayout
{
    /// <summary>Default border radius.</summary>
    public string BorderRadius { get; init; } = "12px";

    /// <summary>Appbar elevation (0-24).</summary>
    public int AppbarElevation { get; init; } = 1;

    /// <summary>Default drawer width.</summary>
    public int DrawerWidth { get; init; } = 240;
}
