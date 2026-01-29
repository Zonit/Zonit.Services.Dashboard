using System.Reflection;

namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Provides read-only access to dashboard settings.
/// Use this interface in components that need to read settings.
/// </summary>
public interface IDashboardSettingsProvider
{
    /// <summary>
    /// Gets the current dashboard settings.
    /// </summary>
    DashboardSettings Settings { get; }
}

/// <summary>
/// Manages dashboard settings for the current request scope.
/// Used internally to configure settings per-area.
/// </summary>
public interface IDashboardSettingsManager : IDashboardSettingsProvider
{
    /// <summary>
    /// Gets the additional assemblies to scan for components.
    /// </summary>
    Assembly[] Assemblies { get; }

    /// <summary>
    /// Sets the dashboard settings for the current scope.
    /// </summary>
    /// <param name="settings">The settings to apply.</param>
    void Configure(DashboardSettings settings);

    /// <summary>
    /// Sets the additional assemblies to scan for components.
    /// </summary>
    /// <param name="assemblies">The assemblies to register.</param>
    void SetAssemblies(Assembly[] assemblies);
}
