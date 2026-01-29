using Microsoft.AspNetCore.Components;

namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Base interface for all dashboard extensions.
/// Implement this interface to create a new dashboard extension that can be registered in DI.
/// </summary>
public interface IDashboardExtension
{
    /// <summary>
    /// Gets the unique identifier of the extension.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display name of the extension.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the order in which the extension should be loaded.
    /// Lower values are loaded first.
    /// </summary>
    int Order => 0;

    /// <summary>
    /// Gets a value indicating whether the extension is enabled.
    /// </summary>
    bool IsEnabled { get; }
}

/// <summary>
/// Extension that provides navigation items.
/// </summary>
public interface INavigationExtension : IDashboardExtension
{
    /// <summary>
    /// Gets the navigation items provided by this extension.
    /// </summary>
    /// <param name="context">Navigation context with current area and user information.</param>
    /// <returns>Collection of navigation items.</returns>
    IEnumerable<NavigationItem> GetNavigationItems(DashboardNavigationContext context);
}

/// <summary>
/// Extension that provides a drawer component.
/// AOT-safe: Uses RenderFragment factory instead of Type reflection.
/// </summary>
public interface IDrawerExtension : IDashboardExtension
{
    /// <summary>
    /// Creates the RenderFragment for the drawer content.
    /// This is AOT-safe as it doesn't require reflection.
    /// </summary>
    /// <returns>RenderFragment that renders the drawer content.</returns>
    RenderFragment CreateDrawerContent();

    /// <summary>
    /// Gets the drawer anchor position.
    /// </summary>
    DrawerAnchor Anchor => DrawerAnchor.End;

    /// <summary>
    /// Gets the drawer width in pixels.
    /// </summary>
    int Width => 320;
}

/// <summary>
/// Extension that provides toolbar items.
/// AOT-safe: Uses RenderFragment factory instead of Type reflection.
/// </summary>
public interface IToolbarExtension : IDashboardExtension
{
    /// <summary>
    /// Creates the RenderFragment for the toolbar content.
    /// This is AOT-safe as it doesn't require reflection.
    /// </summary>
    /// <returns>RenderFragment that renders the toolbar content.</returns>
    RenderFragment CreateToolbarContent();

    /// <summary>
    /// Gets the toolbar position (left or right side of the appbar).
    /// </summary>
    ToolbarPosition Position => ToolbarPosition.Right;
}

/// <summary>
/// Extension that provides settings panel.
/// AOT-safe: Uses RenderFragment factory instead of Type reflection.
/// </summary>
public interface ISettingsExtension : IDashboardExtension
{
    /// <summary>
    /// Creates the RenderFragment for the settings content.
    /// This is AOT-safe as it doesn't require reflection.
    /// </summary>
    /// <returns>RenderFragment that renders the settings content.</returns>
    RenderFragment CreateSettingsContent();

    /// <summary>
    /// Gets the settings section name.
    /// </summary>
    string Section { get; }
}
