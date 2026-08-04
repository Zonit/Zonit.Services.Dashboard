using MudBlazor;

namespace Zonit.Dashboard.Themes;

/// <summary>
/// The dashboard's type scale, shared by every built-in theme.
/// </summary>
/// <remarks>
/// <para><b>Why this exists as its own file.</b> Typography is not a per-theme decision — Ocean
/// and Forest differ from Default in palette only, and a copy of this block in each of them would
/// drift. It also has to be impossible to forget: the 10.x rewrite dropped the <c>Typography</c>
/// block while carrying <c>LayoutProperties</c> across, and nothing failed. The dashboard simply
/// inherited MudBlazor's stock Material scale, where <c>Typo.h4</c> is 2.125rem instead of
/// 1.25rem — a page title rendering at 34px where the design called for 20px, on every screen at
/// once, with no error anywhere.</para>
///
/// <para><b>Why the scale is compressed.</b> Material's default headings are sized for marketing
/// pages. An admin screen is dense: it stacks a title, a metric strip, tabs and a table inside one
/// viewport, so h1 starts at 1.75rem and each level steps down by roughly 0.1rem. Headings stay
/// distinguishable by weight and spacing rather than by size.</para>
///
/// <para><b>On the font stack.</b> Inter first, Roboto second. The dashboard shell loads Roboto
/// itself, so the fallback is always present; Inter applies when the host has loaded it (see the
/// <c>CustomSnippet</c> option), which is the normal case for a host with its own brand.</para>
/// </remarks>
internal static class DashboardTypography
{
    private static readonly string[] Stack = ["Inter", "Roboto", "system-ui", "sans-serif"];

    /// <summary>A fresh instance per call — <see cref="MudTheme"/> takes ownership of what it is given.</summary>
    public static Typography Create() => new()
    {
        Default = new DefaultTypography
        {
            FontFamily = Stack, FontSize = "1rem", LineHeight = "1.6", FontWeight = "400",
        },
        H1 = new H1Typography
        {
            FontFamily = Stack, FontSize = "1.75rem", FontWeight = "600",
            LineHeight = "1.5", LetterSpacing = "-0.01em",
        },
        H2 = new H2Typography
        {
            FontFamily = Stack, FontSize = "1.5rem", FontWeight = "600",
            LineHeight = "1.5", LetterSpacing = "-0.01em",
        },
        H3 = new H3Typography
        {
            FontFamily = Stack, FontSize = "1.35rem", FontWeight = "600",
            LineHeight = "1.4", LetterSpacing = "-0.005em",
        },
        H4 = new H4Typography
        {
            FontFamily = Stack, FontSize = "1.25rem", FontWeight = "500",
            LineHeight = "1.4", LetterSpacing = "-0.005em",
        },
        H5 = new H5Typography
        {
            FontFamily = Stack, FontSize = "1.15rem", FontWeight = "500", LineHeight = "1.4",
        },
        H6 = new H6Typography
        {
            FontFamily = Stack, FontSize = "1.05rem", FontWeight = "500", LineHeight = "1.3",
        },
        Subtitle1 = new Subtitle1Typography
        {
            FontFamily = Stack, FontSize = "1rem", FontWeight = "500", LineHeight = "1.6",
        },
        Subtitle2 = new Subtitle2Typography
        {
            FontFamily = Stack, FontSize = "0.9rem", FontWeight = "500", LineHeight = "1.6",
        },
        Body1 = new Body1Typography
        {
            FontFamily = Stack, FontSize = "1rem", FontWeight = "400", LineHeight = "1.6",
        },
        Body2 = new Body2Typography
        {
            FontFamily = Stack, FontSize = "0.9rem", FontWeight = "400", LineHeight = "1.6",
        },
        Button = new ButtonTypography
        {
            FontFamily = Stack, FontSize = "0.95rem", FontWeight = "500",
            LineHeight = "1.5", LetterSpacing = "0.02em",
        },
        Caption = new CaptionTypography
        {
            FontFamily = Stack, FontSize = "0.8rem", FontWeight = "400", LineHeight = "1.4",
        },
        Overline = new OverlineTypography
        {
            FontFamily = Stack, FontSize = "0.75rem", FontWeight = "500",
            LineHeight = "1.4", LetterSpacing = "0.08em", TextTransform = "uppercase",
        },
    };
}
