namespace Zonit.Dashboard;

/// <summary>
/// Per-mount theme overrides layered on top of the tenant theme
/// (<c>Tenant.Settings.Theme</c>) for a single <c>app.UseDashboard(...)</c> call.
/// </summary>
/// <remarks>
/// <para>The use case: a single tenant hosts several dashboards under the same
/// brand (admin / management / diagnostic), and the operator wants chrome that
/// is visibly distinct per mount (e.g. red appbar for the diagnostic panel, blue
/// for admin) without forcing the whole tenant theme to change.</para>
///
/// <para>All properties are nullable — a <see langword="null"/> means
/// "inherit from the tenant theme". Set only the slots that should differ.</para>
///
/// <para>Read by <see cref="IDashboardCurrentSite.ThemeOverrides"/> at render time,
/// merged with <c>Tenant.Settings.Theme</c> inside <c>DefaultDashboardTheme</c>.
/// Custom themes can choose to ignore overrides if they prefer a fixed palette.</para>
/// </remarks>
public sealed class DashboardThemeOverrides
{
    /// <summary>Override for <c>PrimaryColor</c> (brand main).</summary>
    public string? PrimaryColor { get; set; }

    /// <summary>Override for <c>SecondaryColor</c> (brand supporting).</summary>
    public string? SecondaryColor { get; set; }

    /// <summary>Override for <c>AccentColor</c> (highlights / CTAs).</summary>
    public string? AccentColor { get; set; }

    /// <summary>Override for <c>NeutralColor</c> (page background).</summary>
    public string? NeutralColor { get; set; }

    /// <summary>Override for <c>SurfaceColor</c> (cards / panels).</summary>
    public string? SurfaceColor { get; set; }

    /// <summary>Override for <c>ContentColor</c> (primary text).</summary>
    public string? ContentColor { get; set; }

    /// <summary><see langword="true"/> when at least one override is set.</summary>
    public bool HasAny =>
        PrimaryColor is not null ||
        SecondaryColor is not null ||
        AccentColor is not null ||
        NeutralColor is not null ||
        SurfaceColor is not null ||
        ContentColor is not null;
}
