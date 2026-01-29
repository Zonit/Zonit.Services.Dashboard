namespace Zonit.Services.Dashboard;

/// <summary>
/// Manages dashboard extension drawers.
/// </summary>
/// <remarks>
/// This interface is maintained for backward compatibility.
/// New code should use <see cref="Abstractions.IExtensionDrawerManager"/> instead.
/// </remarks>
[Obsolete("Use IExtensionDrawerManager from Zonit.Services.Dashboard.Abstractions namespace instead.")]
public interface IExtensionManager
{
    /// <summary>
    /// Gets the drawer manager for a specific extension.
    /// </summary>
    /// <param name="extension">Extension name.</param>
    /// <returns>Drawer interface.</returns>
    IDrawer Drawer(string extension);
}

/// <summary>
/// Manages a single extension drawer.
/// </summary>
/// <remarks>
/// This interface is maintained for backward compatibility.
/// New code should use <see cref="Abstractions.IExtensionDrawer"/> instead.
/// </remarks>
[Obsolete("Use IExtensionDrawer from Zonit.Services.Dashboard.Abstractions namespace instead.")]
public interface IDrawer
{
    /// <summary>
    /// Gets whether the drawer is open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// Opens the drawer.
    /// </summary>
    void Open();

    /// <summary>
    /// Closes the drawer.
    /// </summary>
    void Close();

    /// <summary>
    /// Toggles the drawer state.
    /// </summary>
    /// <returns>New state (true = open).</returns>
    bool Toggle();

    /// <summary>
    /// Sets the drawer state.
    /// </summary>
    /// <param name="open">Whether to open.</param>
    void Set(bool open);

    /// <summary>
    /// Event raised when state changes.
    /// </summary>
    event Action? OnChange;
}