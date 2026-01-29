using MudBlazor;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard;

/// <summary>
/// Converts dashboard theme to MudBlazor theme.
/// </summary>
public static class ThemeConverter
{
    /// <summary>
    /// Converts a dashboard theme to a MudBlazor MudTheme.
    /// </summary>
    /// <param name="theme">The dashboard theme.</param>
    /// <returns>A configured MudTheme.</returns>
    public static MudTheme ToMudTheme(this IDashboardTheme theme)
    {
        return new MudTheme
        {
            PaletteLight = ToPaletteLight(theme.LightPalette),
            PaletteDark = ToPaletteDark(theme.DarkPalette),
            Typography = ToTypography(theme.Typography),
            LayoutProperties = ToLayoutProperties(theme.Layout)
        };
    }

    /// <summary>
    /// Converts ThemePalette to MudBlazor PaletteLight.
    /// </summary>
    public static PaletteLight ToPaletteLight(ThemePalette palette)
    {
        return new PaletteLight
        {
            Primary = palette.Primary,
            Secondary = palette.Secondary,
            Tertiary = palette.Tertiary,
            Info = palette.Info,
            Success = palette.Success,
            Warning = palette.Warning,
            Error = palette.Error,
            Background = palette.Background,
            Surface = palette.Surface,
            AppbarBackground = palette.AppbarBackground,
            DrawerBackground = palette.DrawerBackground,
            TextPrimary = palette.TextPrimary,
            TextSecondary = palette.TextSecondary,
            TextDisabled = palette.TextDisabled,
            AppbarText = palette.AppbarText,
            DrawerText = palette.DrawerText,
            DrawerIcon = palette.DrawerText,
            ActionDefault = palette.ActionDefault,
            ActionDisabled = palette.TextDisabled,
            ActionDisabledBackground = "rgba(0,0,0,0.12)",
            LinesDefault = palette.Divider,
            LinesInputs = "rgba(0,0,0,0.3)",
            TableLines = palette.Divider,
            TableStriped = "rgba(0,0,0,0.02)",
            Divider = palette.Divider,
            DividerLight = "rgba(0,0,0,0.06)",
            Skeleton = "rgba(0,0,0,0.11)",
            HoverOpacity = palette.HoverOpacity,
            RippleOpacity = palette.RippleOpacity,
            RippleOpacitySecondary = 0.38,
            Dark = "#2a2b3c",
            Black = "#111111",
            BackgroundGray = "#f0f1f5"
        };
    }

    /// <summary>
    /// Converts ThemePalette to MudBlazor PaletteDark.
    /// </summary>
    public static PaletteDark ToPaletteDark(ThemePalette palette)
    {
        return new PaletteDark
        {
            Primary = palette.Primary,
            Secondary = palette.Secondary,
            Tertiary = palette.Tertiary,
            Info = palette.Info,
            Success = palette.Success,
            Warning = palette.Warning,
            Error = palette.Error,
            Background = palette.Background,
            Surface = palette.Surface,
            AppbarBackground = palette.AppbarBackground,
            DrawerBackground = palette.DrawerBackground,
            TextPrimary = palette.TextPrimary,
            TextSecondary = palette.TextSecondary,
            TextDisabled = palette.TextDisabled,
            AppbarText = palette.AppbarText,
            DrawerText = palette.DrawerText,
            DrawerIcon = palette.DrawerText,
            ActionDefault = palette.ActionDefault,
            ActionDisabled = palette.TextDisabled,
            ActionDisabledBackground = "rgba(255,255,255,0.12)",
            LinesDefault = palette.Divider,
            LinesInputs = "rgba(255,255,255,0.3)",
            TableLines = palette.Divider,
            TableStriped = "rgba(255,255,255,0.02)",
            Divider = palette.Divider,
            DividerLight = "rgba(255,255,255,0.06)",
            Skeleton = "rgba(255,255,255,0.11)",
            HoverOpacity = palette.HoverOpacity,
            RippleOpacity = palette.RippleOpacity,
            RippleOpacitySecondary = 0.38,
            Dark = "#27272f",
            Black = "#27272f",
            BackgroundGray = "#1f2033"
        };
    }

    /// <summary>
    /// Converts ThemeTypography to MudBlazor Typography.
    /// </summary>
    public static Typography ToTypography(ThemeTypography typography)
    {
        var fontFamily = typography.FontFamily;

        return new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = fontFamily,
                FontSize = typography.FontSize,
                LineHeight = typography.LineHeight,
                FontWeight = typography.FontWeight
            },
            H1 = new H1Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.75rem",
                FontWeight = typography.HeadingFontWeight,
                LineHeight = "1.5",
                LetterSpacing = "-0.01em"
            },
            H2 = new H2Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.5rem",
                FontWeight = typography.HeadingFontWeight,
                LineHeight = "1.5",
                LetterSpacing = "-0.01em"
            },
            H3 = new H3Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.35rem",
                FontWeight = typography.HeadingFontWeight,
                LineHeight = "1.4",
                LetterSpacing = "-0.005em"
            },
            H4 = new H4Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.25rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = "1.4",
                LetterSpacing = "-0.005em"
            },
            H5 = new H5Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.15rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = "1.4"
            },
            H6 = new H6Typography
            {
                FontFamily = fontFamily,
                FontSize = "1.05rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = "1.3"
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = fontFamily,
                FontSize = "1rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = typography.LineHeight
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontFamily = fontFamily,
                FontSize = "0.9rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = typography.LineHeight
            },
            Body1 = new Body1Typography
            {
                FontFamily = fontFamily,
                FontSize = typography.FontSize,
                FontWeight = typography.FontWeight,
                LineHeight = typography.LineHeight
            },
            Body2 = new Body2Typography
            {
                FontFamily = fontFamily,
                FontSize = "0.9rem",
                FontWeight = typography.FontWeight,
                LineHeight = typography.LineHeight
            },
            Button = new ButtonTypography
            {
                FontFamily = fontFamily,
                FontSize = "0.95rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = "1.5",
                LetterSpacing = "0.02em"
            },
            Caption = new CaptionTypography
            {
                FontFamily = fontFamily,
                FontSize = "0.8rem",
                FontWeight = typography.FontWeight,
                LineHeight = "1.4"
            },
            Overline = new OverlineTypography
            {
                FontFamily = fontFamily,
                FontSize = "0.75rem",
                FontWeight = typography.ButtonFontWeight,
                LineHeight = "1.4",
                LetterSpacing = "0.08em",
                TextTransform = "uppercase"
            }
        };
    }

    /// <summary>
    /// Converts ThemeLayout to MudBlazor LayoutProperties.
    /// </summary>
    public static LayoutProperties ToLayoutProperties(ThemeLayout layout)
    {
        return new LayoutProperties
        {
            DefaultBorderRadius = layout.BorderRadius
        };
    }
}
