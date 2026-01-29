namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Manages dashboard extension drawers.
/// Provides a way to open, close, and toggle drawer panels for extensions.
/// </summary>
public interface IExtensionDrawerManager
{
    /// <summary>
    /// Gets the drawer instance for a specific extension.
    /// </summary>
    /// <param name="extensionId">The extension identifier.</param>
    /// <returns>The drawer instance for the extension.</returns>
    IExtensionDrawer GetDrawer(string extensionId);
}

/// <summary>
/// Represents a single extension drawer panel.
/// </summary>
public interface IExtensionDrawer
{
    /// <summary>
    /// Gets a value indicating whether the drawer is currently open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the drawer. If already open, does nothing.
    /// </summary>
    void Open();

    /// <summary>
    /// Closes the drawer. If already closed, does nothing.
    /// </summary>
    void Close();

    /// <summary>
    /// Toggles the drawer state.
    /// </summary>
    /// <returns>The new state of the drawer (true = open, false = closed).</returns>
    bool Toggle();

    /// <summary>
    /// Sets the drawer state explicitly.
    /// </summary>
    /// <param name="open">True to open, false to close.</param>
    void SetOpen(bool open);

    /// <summary>
    /// Event raised when the drawer state changes.
    /// </summary>
    event Action? OnChange;
}
