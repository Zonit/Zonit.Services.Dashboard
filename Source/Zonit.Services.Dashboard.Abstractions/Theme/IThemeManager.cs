namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Manages theme state and switching for the dashboard.
/// </summary>
public interface IThemeManager
{
    /// <summary>
    /// Gets all registered themes.
    /// </summary>
    IReadOnlyList<IDashboardTheme> Themes { get; }

    /// <summary>
    /// Gets the currently active theme.
    /// </summary>
    IDashboardTheme CurrentTheme { get; }

    /// <summary>
    /// Gets or sets the current theme mode (Light, Dark, Auto).
    /// </summary>
    ThemeMode CurrentMode { get; set; }

    /// <summary>
    /// Gets whether dark mode is currently active.
    /// Takes into account Auto mode and system preference.
    /// </summary>
    bool IsDarkMode { get; }

    /// <summary>
    /// Event fired when theme or mode changes.
    /// </summary>
    event Action? OnThemeChanged;

    /// <summary>
    /// Sets the current theme by ID.
    /// </summary>
    /// <param name="themeId">The theme identifier.</param>
    /// <returns>True if theme was found and set, false otherwise.</returns>
    bool SetTheme(string themeId);

    /// <summary>
    /// Sets the theme mode.
    /// </summary>
    /// <param name="mode">The theme mode.</param>
    void SetMode(ThemeMode mode);

    /// <summary>
    /// Updates dark mode state based on system preference.
    /// Call this after detecting system theme changes.
    /// </summary>
    /// <param name="isSystemDark">True if system prefers dark mode.</param>
    void UpdateSystemDarkMode(bool isSystemDark);

    /// <summary>
    /// Gets the effective palette for the current state.
    /// </summary>
    ThemePalette GetCurrentPalette();
}
