using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application.Services;

/// <summary>
/// Default implementation of <see cref="IExtensionRegistry"/>.
/// Manages registered dashboard extensions.
/// </summary>
internal sealed class ExtensionRegistry : IExtensionRegistry
{
    private readonly IEnumerable<IDashboardExtension> _extensions;
    private readonly IDashboardSettingsProvider _settingsProvider;

    public ExtensionRegistry(
        IEnumerable<IDashboardExtension> extensions,
        IDashboardSettingsProvider settingsProvider)
    {
        _extensions = extensions;
        _settingsProvider = settingsProvider;
    }

    /// <inheritdoc />
    public IReadOnlyList<IDashboardExtension> GetAll()
    {
        return _extensions
            .OrderBy(x => x.Order)
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<IDashboardExtension> GetEnabled()
    {
        var enabledIds = _settingsProvider.Settings.Extensions;
        return _extensions
            .Where(x => x.IsEnabled && enabledIds.Contains(x.Id, StringComparer.OrdinalIgnoreCase))
            .OrderBy(x => x.Order)
            .ToList();
    }

    /// <inheritdoc />
    public IDashboardExtension? Get(string extensionId)
    {
        return _extensions.FirstOrDefault(x =>
            string.Equals(x.Id, extensionId, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public bool IsEnabled(string extensionId)
    {
        var extension = Get(extensionId);
        if (extension is null || !extension.IsEnabled)
            return false;

        return _settingsProvider.Settings.Extensions
            .Contains(extensionId, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public IReadOnlyList<INavigationExtension> GetNavigationExtensions()
    {
        return GetEnabled()
            .OfType<INavigationExtension>()
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<IDrawerExtension> GetDrawerExtensions()
    {
        return GetEnabled()
            .OfType<IDrawerExtension>()
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<IToolbarExtension> GetToolbarExtensions()
    {
        return GetEnabled()
            .OfType<IToolbarExtension>()
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<ISettingsExtension> GetSettingsExtensions()
    {
        return GetEnabled()
            .OfType<ISettingsExtension>()
            .ToList();
    }
}
