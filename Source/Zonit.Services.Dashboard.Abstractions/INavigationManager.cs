namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Manages navigation items for the dashboard.
/// Extensions and application code can register navigation items through this interface.
/// </summary>
public interface INavigationManager
{
    /// <summary>
    /// Raised when navigation items change.
    /// </summary>
    event Action? OnNavigationChanged;

    /// <summary>
    /// Registers a navigation item.
    /// </summary>
    /// <param name="area">The dashboard area.</param>
    /// <param name="item">The navigation item to register.</param>
    /// <param name="position">The position in the layout (left or right drawer).</param>
    void Register(DashboardArea area, NavigationItem item, NavigationPosition position = NavigationPosition.Left);

    /// <summary>
    /// Registers multiple navigation items.
    /// </summary>
    /// <param name="area">The dashboard area.</param>
    /// <param name="items">The navigation items to register.</param>
    /// <param name="position">The position in the layout.</param>
    void Register(DashboardArea area, IEnumerable<NavigationItem> items, NavigationPosition position = NavigationPosition.Left);

    /// <summary>
    /// Gets all navigation items for the specified context.
    /// Items are sorted by Order property.
    /// </summary>
    /// <param name="context">The navigation context with area and position.</param>
    /// <returns>Sorted list of navigation items.</returns>
    IReadOnlyList<NavigationItem> GetNavigation(DashboardNavigationContext context);

    /// <summary>
    /// Gets all navigation items for the specified area and position.
    /// Items are sorted by Order property.
    /// </summary>
    /// <param name="area">The dashboard area.</param>
    /// <param name="position">The position in the layout.</param>
    /// <returns>Sorted list of navigation items.</returns>
    IReadOnlyList<NavigationItem> GetItems(DashboardArea area, NavigationPosition position = NavigationPosition.Left);

    /// <summary>
    /// Clears all registered navigation items.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clears navigation items for a specific area.
    /// </summary>
    /// <param name="area">The dashboard area to clear.</param>
    void Clear(DashboardArea area);

    /// <summary>
    /// Notifies subscribers that navigation has changed.
    /// </summary>
    void NotifyChanged();
}
