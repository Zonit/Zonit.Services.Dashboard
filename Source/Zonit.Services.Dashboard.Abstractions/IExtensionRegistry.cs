namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Provides access to registered dashboard extensions.
/// </summary>
public interface IExtensionRegistry
{
    /// <summary>
    /// Gets all registered extensions.
    /// </summary>
    IReadOnlyList<IDashboardExtension> GetAll();

    /// <summary>
    /// Gets all enabled extensions for the current dashboard settings.
    /// </summary>
    IReadOnlyList<IDashboardExtension> GetEnabled();

    /// <summary>
    /// Gets an extension by its ID.
    /// </summary>
    /// <param name="extensionId">The extension identifier.</param>
    /// <returns>The extension instance or null if not found.</returns>
    IDashboardExtension? Get(string extensionId);

    /// <summary>
    /// Checks if an extension is enabled in the current dashboard settings.
    /// </summary>
    /// <param name="extensionId">The extension identifier.</param>
    /// <returns>True if the extension is enabled.</returns>
    bool IsEnabled(string extensionId);

    /// <summary>
    /// Gets all navigation extensions.
    /// </summary>
    IReadOnlyList<INavigationExtension> GetNavigationExtensions();

    /// <summary>
    /// Gets all drawer extensions.
    /// </summary>
    IReadOnlyList<IDrawerExtension> GetDrawerExtensions();

    /// <summary>
    /// Gets all toolbar extensions.
    /// </summary>
    IReadOnlyList<IToolbarExtension> GetToolbarExtensions();

    /// <summary>
    /// Gets all settings extensions.
    /// </summary>
    IReadOnlyList<ISettingsExtension> GetSettingsExtensions();
}
