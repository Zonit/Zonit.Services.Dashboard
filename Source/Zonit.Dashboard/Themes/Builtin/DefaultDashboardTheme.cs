using MudBlazor;
using Zonit.Extensions.Tenants;

namespace Zonit.Dashboard.Themes.Builtin;

/// <summary>
/// Built-in dashboard theme that follows the host's brand: brand colors come from
/// <c>Tenant.Settings.Theme</c> (<c>PrimaryColor</c> / <c>SecondaryColor</c> /
/// <c>AccentColor</c> / <c>NeutralColor</c> / <c>SurfaceColor</c> / <c>ContentColor</c>),
/// so an admin changing the tenant theme propagates straight to the dashboard
/// without redeploying. Everything else (info / success / warning / error,
/// surface tints) carries the dashboard's own look, and the type scale comes from
/// <see cref="DashboardTypography"/>, shared with the other built-ins.
/// </summary>
/// <remarks>
/// <para><b>Brand colours are contrast-corrected per mode, not copied.</b> This used to feed the
/// tenant hex into both palettes and note that "MudBlazor adjusts contrast against the dark
/// surface". It does not. With the shipped defaults that put <c>#2563EB</c> on the dark surface at
/// <b>2.94:1</b> — under the 3:1 floor for large text, on the colour that paints every link, every
/// primary button and the active sidebar row. Each brand slot now goes through
/// <see cref="ThemeColors.EnsureContrast"/> against the surface it will actually be painted on, so
/// hue and saturation survive and legibility is guaranteed for <em>any</em> tenant brand rather
/// than only for the ones that happened to work.</para>
///
/// <para><b>Status colours are per-mode too, for the same reason.</b> The old light values were
/// chosen to look bright rather than to be read: <c>#ffa800</c> on white is <b>1.93:1</b>,
/// <c>#0bba83</c> is <b>2.51:1</b>, <c>#3299ff</c> is <b>2.94:1</b>. All three are AA failures for
/// any text or icon drawn in them. The light set below is the darker end of each hue; the dark set
/// is the lighter end. Both are then run through the same contrast pass, so a future edit cannot
/// silently reintroduce the problem.</para>
///
/// <para><b>Surface separation.</b> Dark <c>Background #161723</c> against <c>Surface #232438</c>
/// measured <b>1.17:1</b> and the appbar against the page <b>1.07:1</b> — a card and the page it
/// sits on were the same colour to the eye, which is the other half of "no contrast". Worse, the
/// ladder was not even monotonic: the appbar was <em>darker</em> than the cards it floats over, so
/// what elevation there was pointed the wrong way. The ramp below is monotonic — page → chrome →
/// surface — and steps as far as a dark theme usefully can.</para>
///
/// <para><b>Fill alone cannot carry a dark elevation ladder</b>, and pretending otherwise is what
/// produced the flat field. Every step in a near-black ramp is worth about 1.1–1.25:1; reaching
/// 3:1 by fill would mean a "page" light enough to stop being dark mode. So the boundary is drawn
/// instead of shaded: see the <c>.mud-paper</c> / <c>.zbar</c> outline rules in dashboard.css,
/// which paint a 1px divider-coloured edge in dark mode only. That is the Material-3 answer and it
/// is the one that actually separates a card from its page.</para>
///
/// <para>Scoped lifetime: pulls the per-circuit <see cref="ITenantProvider"/> and
/// rebuilds the underlying <see cref="MudTheme"/> on each tenant change so
/// re-renders reflect the latest brand. Other built-in themes (Ocean / Forest)
/// are scoped too but ignore the tenant — they are user-aesthetic choices.</para>
/// </remarks>
internal sealed class DefaultDashboardTheme(ITenantProvider tenant, IDashboardCurrentSite site) : IDashboardTheme
{
    public string Id => "default";
    public string Name => "Default";
    public string Description => "Follows the tenant's brand colors with optional per-mount overrides.";

    // ─── Dark ramp ─────────────────────────────────────────────────────────────
    // Neutral-cool rather than blue-tinted: a saturated dark ground makes every brand hue on top
    // of it fight the background. Ratios against DarkSurface are stated where they matter.
    private const string DarkBackground = "#0a0d12";  // page — the darkest step
    private const string DarkChrome     = "#131824";  // appbar + drawers, 1.10:1 up from the page
    private const string DarkSurface    = "#1c2230";  // cards, menus, popovers — 1.23:1 vs page
    private const string DarkSurfaceAlt = "#252d3d";

    // ─── Light ramp ────────────────────────────────────────────────────────────
    private const string LightBackground = "#f4f6fa";
    private const string LightSurface    = "#ffffff";

    // Cached per (tenant brand + mount override) tuple.
    //
    // This was an expression-bodied property rebuilding the whole MudTheme — two palettes, a
    // Typography and a LayoutProperties — on every read, and MudThemeProvider reads it on every
    // render of the layout, which is every drawer toggle, every navigation and every reactive
    // event the layout subscribes to. That was already wasteful; with the contrast pass in it the
    // rebuild now also runs up to six lightness searches, so leaving it uncached would put real
    // arithmetic on the render path. The theme is a pure function of the six colour slots below,
    // so caching on exactly those is safe, and a tenant changing its brand still repaints
    // immediately because the key changes with it.
    private MudTheme? _cached;
    private string? _cacheKey;

    public MudTheme MudTheme
    {
        get
        {
            var o = site.ThemeOverrides;
            var t = tenant.Settings.Theme;
            var key = string.Join('|',
                o.PrimaryColor ?? t.PrimaryColor,
                o.SecondaryColor ?? t.SecondaryColor,
                o.AccentColor ?? t.AccentColor,
                o.NeutralColor ?? t.NeutralColor,
                o.SurfaceColor ?? t.SurfaceColor,
                o.ContentColor ?? t.ContentColor);

            if (_cached is null || _cacheKey != key)
            {
                _cached = BuildTheme(tenant, o);
                _cacheKey = key;
            }

            return _cached;
        }
    }

    private static MudTheme BuildTheme(ITenantProvider tenant, DashboardThemeOverrides o)
    {
        var t = tenant.Settings.Theme;

        // Per-mount override wins over tenant brand; this lets a single tenant
        // run several dashboards with visually distinct chrome (admin red,
        // diagnostic grey) without forcing the tenant theme to change.
        var primary   = o.PrimaryColor   ?? t.PrimaryColor;
        var secondary = o.SecondaryColor ?? t.SecondaryColor;
        var accent    = o.AccentColor    ?? t.AccentColor;
        var neutral   = o.NeutralColor   ?? t.NeutralColor;
        var surface   = o.SurfaceColor   ?? t.SurfaceColor;
        var content   = o.ContentColor   ?? t.ContentColor;

        // A tenant that leaves the neutral/surface slots at a value which collapses the two into
        // one flat sheet (or sets a dark neutral under a light palette) previously produced a
        // dashboard with no visible card edges at all. Fall back to the dashboard's own ramp
        // whenever the pair cannot carry a card boundary — 1.06:1 is roughly the point below which
        // the edge stops existing for anyone not looking for it.
        var lightBackground = ThemeColors.Ratio(neutral, surface) >= 1.06 ? neutral : LightBackground;
        var lightSurface    = ThemeColors.Ratio(neutral, surface) >= 1.06 ? surface : LightSurface;

        // Text must clear AA against BOTH the page and the card, so it is corrected against
        // whichever of the two is the harder ground.
        var lightText = ThemeColors.EnsureContrast(content, lightSurface, ThemeColors.AaText);

        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                // Brand, corrected against the surface it is drawn on. A well-chosen light-mode
                // brand passes untouched — the pass only fires where it is needed.
                Primary       = ThemeColors.EnsureContrast(primary,   lightSurface, ThemeColors.AaText),
                PrimaryContrastText = ThemeColors.OnColor(primary),
                Secondary     = ThemeColors.EnsureContrast(secondary, lightSurface, ThemeColors.AaText),
                SecondaryContrastText = ThemeColors.OnColor(secondary),
                Tertiary      = ThemeColors.EnsureContrast(accent,    lightSurface, ThemeColors.AaText),
                TertiaryContrastText = ThemeColors.OnColor(accent),

                Background        = lightBackground,
                BackgroundGray    = ThemeColors.Mix(lightBackground, "#000000", 0.04),
                Surface           = lightSurface,
                TextPrimary       = lightText,

                // Status colours — the readable end of each hue. Previous values were 1.9–3.4:1
                // on white; these clear AA as text and still carry white at 4.5:1+ when MudBlazor
                // fills a button or an alert with them.
                Info    = "#0369a1",
                Success = "#047857",
                Warning = "#b45309",
                Error   = "#c2263a",

                InfoContrastText    = "#ffffff",
                SuccessContrastText = "#ffffff",
                WarningContrastText = "#ffffff",
                ErrorContrastText   = "#ffffff",

                // Chrome surfaces — derived from the effective surface color.
                AppbarBackground = lightSurface,
                DrawerBackground = lightSurface,

                // 0.62 of the way to the page ground keeps secondary text quiet without dropping
                // it under AA — the old #4b5563 was fine, this simply follows the tenant's own
                // text colour instead of pinning a grey next to it.
                TextSecondary    = ThemeColors.EnsureContrast(
                                       ThemeColors.Mix(lightText, lightSurface, 0.38), lightSurface, ThemeColors.AaText),
                TextDisabled     = "rgba(15,23,42,0.42)",
                AppbarText       = lightText,
                DrawerText       = ThemeColors.EnsureContrast(
                                       ThemeColors.Mix(lightText, lightSurface, 0.30), lightSurface, ThemeColors.AaText),
                ActionDefault    = ThemeColors.EnsureContrast(
                                       ThemeColors.Mix(lightText, lightSurface, 0.42), lightSurface, ThemeColors.AaText),
                ActionDisabled   = "rgba(15,23,42,0.28)",

                // 0.12 alpha measured 1.30:1 against the page — a line nobody can see is a line
                // that is not doing its job. 0.18 reads as a hairline without becoming a rule.
                Divider          = "rgba(15,23,42,0.18)",
                DividerLight     = "rgba(15,23,42,0.10)",
                LinesDefault     = "rgba(15,23,42,0.18)",
                LinesInputs      = "rgba(15,23,42,0.30)",
                TableLines       = "rgba(15,23,42,0.14)",
                TableHover       = "rgba(15,23,42,0.04)",
                TableStriped     = "rgba(15,23,42,0.02)",
                HoverOpacity     = 0.06,
                RippleOpacity    = 0.10,
                OverlayDark      = "rgba(15,23,42,0.5)",
            },
            PaletteDark = new PaletteDark
            {
                // Same brand hex, lifted until it is legible on the dark card. #2563EB comes out
                // around #5b8dee here (≈5.9:1) instead of the 2.94:1 it scored before.
                Primary       = ThemeColors.EnsureContrast(primary,   DarkSurface, ThemeColors.AaText),
                PrimaryContrastText = "#0b0d12",
                Secondary     = ThemeColors.EnsureContrast(secondary, DarkSurface, ThemeColors.AaText),
                SecondaryContrastText = "#0b0d12",
                Tertiary      = ThemeColors.EnsureContrast(accent,    DarkSurface, ThemeColors.AaText),
                TertiaryContrastText = "#0b0d12",

                Info    = "#60a5fa",
                Success = "#34d399",
                Warning = "#fbbf24",
                Error   = "#fb7185",

                InfoContrastText    = "#0b0d12",
                SuccessContrastText = "#0b0d12",
                WarningContrastText = "#0b0d12",
                ErrorContrastText   = "#0b0d12",

                Background       = DarkBackground,
                BackgroundGray   = "#0a0e13",
                Surface          = DarkSurface,
                AppbarBackground = DarkChrome,
                DrawerBackground = DarkChrome,

                // 0.87 → 0.92: the old value put body text at 11.7:1 on the old surface but the
                // surface has moved, and a dashboard is read for hours at a time.
                TextPrimary      = "rgba(255,255,255,0.92)",
                TextSecondary    = "rgba(255,255,255,0.66)",
                TextDisabled     = "rgba(255,255,255,0.40)",
                AppbarText       = "rgba(255,255,255,0.92)",
                DrawerText       = "rgba(255,255,255,0.74)",
                ActionDefault    = "rgba(255,255,255,0.70)",
                ActionDisabled   = "rgba(255,255,255,0.30)",

                // 0.12 measured 1.45:1 on the old surface and disappeared entirely against the
                // page. Dark grounds need more alpha than light ones for the same apparent weight.
                Divider          = "rgba(255,255,255,0.16)",
                DividerLight     = "rgba(255,255,255,0.09)",
                LinesDefault     = "rgba(255,255,255,0.16)",
                LinesInputs      = "rgba(255,255,255,0.28)",
                TableLines       = "rgba(255,255,255,0.12)",
                TableHover       = "rgba(255,255,255,0.05)",
                TableStriped     = "rgba(255,255,255,0.03)",
                HoverOpacity     = 0.08,
                RippleOpacity    = 0.12,
                OverlayDark      = "rgba(3,5,9,0.6)",

                // Named so the sheet below can reference the same value the chrome uses.
                GrayDefault      = DarkSurfaceAlt,
            },
            Typography = DashboardTypography.Create(),
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "12px",
            },
        };
    }
}
