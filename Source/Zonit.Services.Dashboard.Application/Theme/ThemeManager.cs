using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application;

/// <summary>
/// Default implementation of theme manager.
/// Manages theme state and switching.
/// </summary>
public sealed class ThemeManager : IThemeManager
{
    private readonly List<IDashboardTheme> _themes;
    private IDashboardTheme _currentTheme;
    private ThemeMode _currentMode = ThemeMode.Auto;
    private bool _isSystemDarkMode;

    /// <summary>
    /// Creates a new instance of ThemeManager with the specified themes.
    /// </summary>
    /// <param name="themes">Available themes.</param>
    public ThemeManager(IEnumerable<IDashboardTheme> themes)
    {
        _themes = themes.ToList();

        if (_themes.Count == 0)
        {
            _themes.Add(new DefaultDashboardTheme());
        }

        _currentTheme = _themes[0];
    }

    /// <inheritdoc />
    public IReadOnlyList<IDashboardTheme> Themes => _themes.AsReadOnly();

    /// <inheritdoc />
    public IDashboardTheme CurrentTheme => _currentTheme;

    /// <inheritdoc />
    public ThemeMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != value)
            {
                _currentMode = value;
                OnThemeChanged?.Invoke();
            }
        }
    }

    /// <inheritdoc />
    public bool IsDarkMode => _currentMode switch
    {
        ThemeMode.Light => false,
        ThemeMode.Dark => true,
        ThemeMode.Auto => _isSystemDarkMode,
        _ => false
    };

    /// <inheritdoc />
    public event Action? OnThemeChanged;

    /// <inheritdoc />
    public bool SetTheme(string themeId)
    {
        var theme = _themes.FirstOrDefault(t => t.Id == themeId);
        if (theme is null)
            return false;

        if (_currentTheme.Id != theme.Id)
        {
            _currentTheme = theme;
            OnThemeChanged?.Invoke();
        }

        return true;
    }

    /// <inheritdoc />
    public void SetMode(ThemeMode mode)
    {
        CurrentMode = mode;
    }

    /// <inheritdoc />
    public void UpdateSystemDarkMode(bool isSystemDark)
    {
        if (_isSystemDarkMode != isSystemDark)
        {
            _isSystemDarkMode = isSystemDark;
            if (_currentMode == ThemeMode.Auto)
            {
                OnThemeChanged?.Invoke();
            }
        }
    }

    /// <inheritdoc />
    public ThemePalette GetCurrentPalette()
    {
        return IsDarkMode && _currentTheme.SupportsDarkMode
            ? _currentTheme.DarkPalette
            : _currentTheme.LightPalette;
    }
}
