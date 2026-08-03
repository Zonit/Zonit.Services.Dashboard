namespace Zonit.Dashboard.Themes;

/// <summary>
/// User-facing light / dark preference for the dashboard chrome. Mirrors the
/// idiomatic OS / browser <c>prefers-color-scheme</c> tri-state.
/// </summary>
/// <remarks>
/// <para>Persisted per user (cookie) — independent from the host-side
/// <c>Tenant.Settings.Theme</c> defaults, which describe <em>brand</em> colors that
/// apply to both modes. <see cref="ThemeMode"/> selects which palette
/// (<c>PaletteLight</c> / <c>PaletteDark</c>) MudBlazor surfaces; the brand colors
/// inside both palettes still come from the tenant.</para>
/// </remarks>
public enum ThemeMode
{
    /// <summary>Follow the OS / browser <c>prefers-color-scheme</c>. Default for new users.</summary>
    Auto,

    /// <summary>Force the light palette regardless of OS preference.</summary>
    Light,

    /// <summary>Force the dark palette regardless of OS preference.</summary>
    Dark,
}
