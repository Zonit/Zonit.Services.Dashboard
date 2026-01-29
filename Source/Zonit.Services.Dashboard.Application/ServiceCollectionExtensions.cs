#pragma warning disable CS0618 // Type or member is obsolete - Legacy support

using Microsoft.Extensions.DependencyInjection;
using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.Dashboard.Application.Services;

namespace Zonit.Services.Dashboard.Application;

/// <summary>
/// Extension methods for registering dashboard application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds dashboard application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDashboardApplication(this IServiceCollection services)
    {
        // Settings management
        services.AddScoped<DashboardSettingsManager>();
        services.AddScoped<IDashboardSettingsManager>(sp => sp.GetRequiredService<DashboardSettingsManager>());
        services.AddScoped<IDashboardSettingsProvider>(sp => sp.GetRequiredService<DashboardSettingsManager>());

        // Extension management
        services.AddScoped<IExtensionDrawerManager, ExtensionDrawerManager>();
        services.AddScoped<IExtensionRegistry, ExtensionRegistry>();

        // Navigation
        services.AddSingleton<INavigationManager, NavigationManager>();

        // Theme management - register default themes
        services.AddSingleton<IDashboardTheme, DefaultDashboardTheme>();
        services.AddSingleton<IDashboardTheme, OceanDashboardTheme>();
        services.AddSingleton<IDashboardTheme, ForestDashboardTheme>();
        services.AddScoped<IThemeManager, ThemeManager>();

        // Legacy support - register old interface that forwards to new one
        services.AddScoped<IExtensionManager>(sp =>
        {
            var drawerManager = sp.GetRequiredService<IExtensionDrawerManager>();
            return new LegacyExtensionManagerAdapter(drawerManager);
        });

        return services;
    }
}

/// <summary>
/// Adapter to maintain backward compatibility with old IExtensionManager interface.
/// </summary>
internal sealed class LegacyExtensionManagerAdapter : IExtensionManager
{
    private readonly IExtensionDrawerManager _drawerManager;

    public LegacyExtensionManagerAdapter(IExtensionDrawerManager drawerManager)
    {
        _drawerManager = drawerManager;
    }

    public IDrawer Drawer(string extension)
    {
        var drawer = _drawerManager.GetDrawer(extension);
        return new LegacyDrawerAdapter(drawer);
    }
}

/// <summary>
/// Adapter for legacy IDrawer interface.
/// </summary>
internal sealed class LegacyDrawerAdapter : IDrawer
{
    private readonly IExtensionDrawer _drawer;

    public LegacyDrawerAdapter(IExtensionDrawer drawer)
    {
        _drawer = drawer;
    }

    public bool IsOpen => _drawer.IsOpen;

    public event Action? OnChange
    {
        add => _drawer.OnChange += value;
        remove => _drawer.OnChange -= value;
    }

    public void Open() => _drawer.Open();
    public void Close() => _drawer.Close();
    public bool Toggle() => _drawer.Toggle();
    public void Set(bool open) => _drawer.SetOpen(open);
}

#pragma warning restore CS0618