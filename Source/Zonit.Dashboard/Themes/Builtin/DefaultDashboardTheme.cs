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

    public MudTheme MudTheme => BuildTheme(tenant, site.ThemeOverrides);

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

        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                // Brand — tenant default, mount override layered on top.
                Primary   = primary,
                Secondary = secondary,
                Tertiary  = accent,

                // Neutrals — tenant default + mount override.
                Background = neutral,
                Surface    = surface,
                TextPrimary = content,

                // Status colors — dashboard defaults (independent of tenant brand).
                Info    = "#3299ff",
                Success = "#0bba83",
                Warning = "#ffa800",
                Error   = "#f64e62",

                // Chrome surfaces — derived from the effective surface color.
                AppbarBackground = surface,
                DrawerBackground = surface,
                TextSecondary    = "#4b5563",
                TextDisabled     = "rgba(0,0,0,0.38)",
                AppbarText       = "#424c5f",
                DrawerText       = "#4b5563",
                ActionDefault    = "#64748b",
                Divider          = "rgba(0,0,0,0.12)",
                HoverOpacity     = 0.06,
                RippleOpacity    = 0.10,
            },
            PaletteDark = new PaletteDark
            {
                // Brand — same tenant + mount override stack; MudBlazor adjusts
                // contrast against the dark surface, so the same hex usually reads
                // fine in both modes.
                Primary   = primary,
                Secondary = secondary,
                Tertiary  = accent,

                Info    = "#4dabff",
                Success = "#22d69a",
                Warning = "#ffc233",
                Error   = "#f87785",

                Background       = "#161723",
                Surface          = "#232438",
                AppbarBackground = "#1c1d30",
                DrawerBackground = "#1c1d30",
                TextPrimary      = "rgba(255,255,255,0.87)",
                TextSecondary    = "rgba(255,255,255,0.60)",
                TextDisabled     = "rgba(255,255,255,0.38)",
                AppbarText       = "rgba(255,255,255,0.87)",
                DrawerText       = "rgba(255,255,255,0.70)",
                ActionDefault    = "#b0b3c9",
                Divider          = "rgba(255,255,255,0.12)",
                HoverOpacity     = 0.08,
                RippleOpacity    = 0.12,
            },
            Typography = DashboardTypography.Create(),
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "12px",
            },
        };
    }
}
