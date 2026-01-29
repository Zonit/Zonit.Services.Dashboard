namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Dashboard configuration settings.
/// Configure via DI when registering dashboard areas.
/// </summary>
public class DashboardSettings
{
    /// <summary>
    /// Gets or sets the URL directory segment for this dashboard instance.
    /// Default: "dashboard". Must be lowercase, no leading slash.
    /// </summary>
    /// <example>
    /// For "manager", the dashboard will be available at /manager/
    /// </example>
    public string Directory
    {
        get => _directory;
        init => _directory = (value ?? "dashboard").ToLowerInvariant();
    }
    private readonly string _directory = "dashboard";

    /// <summary>
    /// Gets or sets the display title shown in the header and browser tab.
    /// </summary>
    public string Title { get; init; } = "Dashboard";

    /// <summary>
    /// Gets or sets the authorization policy required to access this dashboard.
    /// Null means any authenticated user can access.
    /// </summary>
    public string? Permission { get; init; }

    /// <summary>
    /// Gets or sets the list of enabled extension IDs.
    /// Extensions not in this list will not be rendered.
    /// </summary>
    public string[] Extensions { get; init; } = [];

    /// <summary>
    /// Gets or sets the dashboard area type.
    /// </summary>
    public DashboardArea Area { get; init; } = DashboardArea.Manager;

    /// <summary>
    /// Gets or sets custom HTML/JS snippet to inject in the page.
    /// Use for analytics, custom scripts, etc.
    /// </summary>
    public string? CustomSnippet { get; init; }

    /// <summary>
    /// Gets or sets whether to enable CSRF antiforgery protection.
    /// Default: true.
    /// </summary>
    public bool EnableAntiforgery { get; init; } = true;

    /// <summary>
    /// Gets or sets the theme color for the dashboard.
    /// Used in meta tags and browser UI.
    /// </summary>
    public string ThemeColor { get; init; } = "#7764e5";

    /// <summary>
    /// Gets or sets the favicon path.
    /// </summary>
    public string FavIcon { get; init; } = "favicon.svg";

    /// <summary>
    /// Gets or sets the theme configuration.
    /// </summary>
    public DashboardThemeSettings Theme { get; init; } = new();

    /// <summary>
    /// Gets or sets the layout configuration.
    /// </summary>
    public DashboardLayoutSettings Layout { get; init; } = new();
}

/// <summary>
/// Theme settings for the dashboard.
/// </summary>
public class DashboardThemeSettings
{
    /// <summary>
    /// Gets or sets the primary color (hex format).
    /// </summary>
    public string Primary { get; init; } = "#7764e5";

    /// <summary>
    /// Gets or sets the secondary color (hex format).
    /// </summary>
    public string Secondary { get; init; } = "#38bdf8";

    /// <summary>
    /// Gets or sets the tertiary/accent color (hex format).
    /// </summary>
    public string Tertiary { get; init; } = "#9333ea";

    /// <summary>
    /// Gets or sets the default theme mode.
    /// </summary>
    public ThemeMode DefaultMode { get; init; } = ThemeMode.Auto;

    /// <summary>
    /// Gets or sets whether users can change the theme.
    /// </summary>
    public bool AllowUserThemeChange { get; init; } = true;

    /// <summary>
    /// Gets or sets custom font family.
    /// Default uses system fonts.
    /// </summary>
    public string? FontFamily { get; init; }

    /// <summary>
    /// Gets or sets the border radius in pixels.
    /// </summary>
    public int BorderRadius { get; init; } = 12;
}

/// <summary>
/// Layout settings for the dashboard.
/// </summary>
public class DashboardLayoutSettings
{
    /// <summary>
    /// Gets or sets whether the left drawer is visible.
    /// </summary>
    public bool ShowLeftDrawer { get; init; } = true;

    /// <summary>
    /// Gets or sets whether the right drawer is visible.
    /// </summary>
    public bool ShowRightDrawer { get; init; } = true;

    /// <summary>
    /// Gets or sets the left drawer width in pixels.
    /// </summary>
    public int LeftDrawerWidth { get; init; } = 240;

    /// <summary>
    /// Gets or sets the right drawer width in pixels.
    /// </summary>
    public int RightDrawerWidth { get; init; } = 280;

    /// <summary>
    /// Gets or sets whether to show breadcrumbs.
    /// </summary>
    public bool ShowBreadcrumbs { get; init; } = true;

    /// <summary>
    /// Gets or sets whether to show the progress bar during tasks.
    /// </summary>
    public bool ShowProgressBar { get; init; } = true;

    /// <summary>
    /// Gets or sets whether to allow drawer swiping on mobile.
    /// </summary>
    public bool EnableSwipeGestures { get; init; } = true;

    /// <summary>
    /// Gets or sets the appbar elevation (0-24).
    /// </summary>
    public int AppbarElevation { get; init; } = 1;
}

/// <summary>
/// Theme mode options.
/// </summary>
public enum ThemeMode
{
    /// <summary>Follow system preference.</summary>
    Auto,
    /// <summary>Always light theme.</summary>
    Light,
    /// <summary>Always dark theme.</summary>
    Dark
}
