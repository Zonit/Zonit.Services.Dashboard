using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application;

/// <summary>
/// Default dashboard theme with purple accent.
/// </summary>
public sealed class DefaultDashboardTheme : IDashboardTheme
{
    /// <inheritdoc />
    public string Id => "default";

    /// <inheritdoc />
    public string Name => "Default";

    /// <inheritdoc />
    public string? Description => "Default purple theme with modern styling.";

    /// <inheritdoc />
    public bool SupportsDarkMode => true;

    /// <inheritdoc />
    public ThemePalette LightPalette { get; } = new()
    {
        Primary = "#7764e5",
        Secondary = "#38bdf8",
        Tertiary = "#9333ea",
        Info = "#3299ff",
        Success = "#0bba83",
        Warning = "#ffa800",
        Error = "#f64e62",
        Background = "#f8f9fc",
        Surface = "#ffffff",
        AppbarBackground = "#ffffff",
        DrawerBackground = "#ffffff",
        TextPrimary = "#1f2937",
        TextSecondary = "#4b5563",
        TextDisabled = "rgba(0,0,0,0.38)",
        AppbarText = "#424c5f",
        DrawerText = "#4b5563",
        ActionDefault = "#64748b",
        Divider = "rgba(0,0,0,0.12)",
        HoverOpacity = 0.06,
        RippleOpacity = 0.10
    };

    /// <inheritdoc />
    public ThemePalette DarkPalette { get; } = new()
    {
        Primary = "#8a7deb",
        Secondary = "#56c8fa",
        Tertiary = "#a855f7",
        Info = "#4dabff",
        Success = "#22d69a",
        Warning = "#ffc233",
        Error = "#f87785",
        Background = "#161723",
        Surface = "#232438",
        AppbarBackground = "#1c1d30",
        DrawerBackground = "#1c1d30",
        TextPrimary = "rgba(255,255,255,0.87)",
        TextSecondary = "rgba(255,255,255,0.60)",
        TextDisabled = "rgba(255,255,255,0.38)",
        AppbarText = "rgba(255,255,255,0.87)",
        DrawerText = "rgba(255,255,255,0.70)",
        ActionDefault = "#b0b3c9",
        Divider = "rgba(255,255,255,0.12)",
        HoverOpacity = 0.08,
        RippleOpacity = 0.12
    };

    /// <inheritdoc />
    public ThemeTypography Typography { get; } = new()
    {
        FontFamily = ["Inter", "Roboto", "system-ui", "sans-serif"],
        FontSize = "1rem",
        LineHeight = "1.6",
        FontWeight = "400",
        HeadingFontWeight = "600",
        ButtonFontWeight = "500"
    };

    /// <inheritdoc />
    public ThemeLayout Layout { get; } = new()
    {
        BorderRadius = "12px",
        AppbarElevation = 1,
        DrawerWidth = 240
    };
}

/// <summary>
/// Ocean blue theme.
/// </summary>
public sealed class OceanDashboardTheme : IDashboardTheme
{
    /// <inheritdoc />
    public string Id => "ocean";

    /// <inheritdoc />
    public string Name => "Ocean";

    /// <inheritdoc />
    public string? Description => "Cool ocean blue theme.";

    /// <inheritdoc />
    public bool SupportsDarkMode => true;

    /// <inheritdoc />
    public ThemePalette LightPalette { get; } = new()
    {
        Primary = "#0284c7",
        Secondary = "#0891b2",
        Tertiary = "#0ea5e9",
        Info = "#38bdf8",
        Success = "#10b981",
        Warning = "#f59e0b",
        Error = "#ef4444",
        Background = "#f0f9ff",
        Surface = "#ffffff",
        AppbarBackground = "#ffffff",
        DrawerBackground = "#ffffff",
        TextPrimary = "#0c4a6e",
        TextSecondary = "#0369a1",
        TextDisabled = "rgba(0,0,0,0.38)",
        AppbarText = "#075985",
        DrawerText = "#0369a1",
        ActionDefault = "#0284c7",
        Divider = "rgba(0,0,0,0.12)",
        HoverOpacity = 0.06,
        RippleOpacity = 0.10
    };

    /// <inheritdoc />
    public ThemePalette DarkPalette { get; } = new()
    {
        Primary = "#38bdf8",
        Secondary = "#22d3ee",
        Tertiary = "#7dd3fc",
        Info = "#67e8f9",
        Success = "#34d399",
        Warning = "#fbbf24",
        Error = "#f87171",
        Background = "#0c1929",
        Surface = "#0f2740",
        AppbarBackground = "#0d1f33",
        DrawerBackground = "#0d1f33",
        TextPrimary = "rgba(255,255,255,0.87)",
        TextSecondary = "rgba(255,255,255,0.60)",
        TextDisabled = "rgba(255,255,255,0.38)",
        AppbarText = "rgba(255,255,255,0.87)",
        DrawerText = "rgba(255,255,255,0.70)",
        ActionDefault = "#7dd3fc",
        Divider = "rgba(255,255,255,0.12)",
        HoverOpacity = 0.08,
        RippleOpacity = 0.12
    };

    /// <inheritdoc />
    public ThemeTypography Typography { get; } = new();

    /// <inheritdoc />
    public ThemeLayout Layout { get; } = new();
}

/// <summary>
/// Forest green theme.
/// </summary>
public sealed class ForestDashboardTheme : IDashboardTheme
{
    /// <inheritdoc />
    public string Id => "forest";

    /// <inheritdoc />
    public string Name => "Forest";

    /// <inheritdoc />
    public string? Description => "Nature-inspired green theme.";

    /// <inheritdoc />
    public bool SupportsDarkMode => true;

    /// <inheritdoc />
    public ThemePalette LightPalette { get; } = new()
    {
        Primary = "#059669",
        Secondary = "#10b981",
        Tertiary = "#34d399",
        Info = "#3b82f6",
        Success = "#22c55e",
        Warning = "#f59e0b",
        Error = "#ef4444",
        Background = "#f0fdf4",
        Surface = "#ffffff",
        AppbarBackground = "#ffffff",
        DrawerBackground = "#ffffff",
        TextPrimary = "#14532d",
        TextSecondary = "#166534",
        TextDisabled = "rgba(0,0,0,0.38)",
        AppbarText = "#15803d",
        DrawerText = "#166534",
        ActionDefault = "#059669",
        Divider = "rgba(0,0,0,0.12)",
        HoverOpacity = 0.06,
        RippleOpacity = 0.10
    };

    /// <inheritdoc />
    public ThemePalette DarkPalette { get; } = new()
    {
        Primary = "#34d399",
        Secondary = "#4ade80",
        Tertiary = "#6ee7b7",
        Info = "#60a5fa",
        Success = "#4ade80",
        Warning = "#fbbf24",
        Error = "#f87171",
        Background = "#0a1f13",
        Surface = "#0f2d1a",
        AppbarBackground = "#0c2516",
        DrawerBackground = "#0c2516",
        TextPrimary = "rgba(255,255,255,0.87)",
        TextSecondary = "rgba(255,255,255,0.60)",
        TextDisabled = "rgba(255,255,255,0.38)",
        AppbarText = "rgba(255,255,255,0.87)",
        DrawerText = "rgba(255,255,255,0.70)",
        ActionDefault = "#6ee7b7",
        Divider = "rgba(255,255,255,0.12)",
        HoverOpacity = 0.08,
        RippleOpacity = 0.12
    };

    /// <inheritdoc />
    public ThemeTypography Typography { get; } = new();

    /// <inheritdoc />
    public ThemeLayout Layout { get; } = new();
}
