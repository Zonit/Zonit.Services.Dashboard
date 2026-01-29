using System.Reflection;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application.Services;

/// <summary>
/// Default implementation of <see cref="IDashboardSettingsManager"/>.
/// Stores settings for the current request scope.
/// </summary>
internal sealed class DashboardSettingsManager : IDashboardSettingsManager
{
    private DashboardSettings _settings = new();
    private Assembly[] _assemblies = [];

    /// <inheritdoc />
    public DashboardSettings Settings => _settings;

    /// <inheritdoc />
    public Assembly[] Assemblies => _assemblies;

    /// <inheritdoc />
    public void Configure(DashboardSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    /// <inheritdoc />
    public void SetAssemblies(Assembly[] assemblies)
    {
        _assemblies = assemblies ?? [];
    }
}
