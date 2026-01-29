namespace Zonit.Services.Dashboard.Abstractions;

/// <summary>
/// Represents a navigation item in the dashboard menu.
/// Supports multi-level nesting for complex menu structures.
/// </summary>
public class NavigationItem
{
    /// <summary>
    /// Gets or sets the display title of the navigation item.
    /// Will be translated using the culture provider.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the URL to navigate to.
    /// Null for group headers that contain children.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Gets or sets the icon to display (MudBlazor icon string or SVG).
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Gets or sets the SVG icon content if using custom SVG.
    /// Takes precedence over Icon property.
    /// </summary>
    public string? SvgIcon { get; init; }

    /// <summary>
    /// Gets or sets the order of this item within its parent.
    /// Lower values appear first.
    /// </summary>
    public int Order { get; init; } = 0;

    /// <summary>
    /// Gets or sets the authorization policy required to view this item.
    /// Null means visible to all authenticated users.
    /// </summary>
    public string? Permission { get; init; }

    /// <summary>
    /// Gets or sets the navigation target (_blank, _self, etc.).
    /// </summary>
    public string? Target { get; init; }

    /// <summary>
    /// Gets or sets whether this link should match exactly (true) or as prefix (false).
    /// </summary>
    public bool ExactMatch { get; init; } = false;

    /// <summary>
    /// Gets or sets the child navigation items for nested menus.
    /// </summary>
    public IReadOnlyList<NavigationItem>? Children { get; init; }

    /// <summary>
    /// Gets or sets the badge text to display on this item.
    /// </summary>
    public string? Badge { get; init; }

    /// <summary>
    /// Gets or sets the badge color.
    /// </summary>
    public NavigationBadgeColor BadgeColor { get; init; } = NavigationBadgeColor.Default;

    /// <summary>
    /// Gets or sets whether the item is disabled.
    /// </summary>
    public bool Disabled { get; init; } = false;

    /// <summary>
    /// Gets or sets a tooltip to display on hover.
    /// </summary>
    public string? Tooltip { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item is a group header.
    /// </summary>
    public bool IsGroup => Children is { Count: > 0 };

    /// <summary>
    /// Gets a value indicating whether this item is expandable (has children that need expanding).
    /// </summary>
    public bool IsExpandable => IsGroup && Url is null;
}

/// <summary>
/// Navigation item badge colors.
/// </summary>
public enum NavigationBadgeColor
{
    /// <summary>Default theme color.</summary>
    Default,
    /// <summary>Primary theme color.</summary>
    Primary,
    /// <summary>Secondary theme color.</summary>
    Secondary,
    /// <summary>Success color (green).</summary>
    Success,
    /// <summary>Warning color (orange).</summary>
    Warning,
    /// <summary>Error color (red).</summary>
    Error,
    /// <summary>Info color (blue).</summary>
    Info
}

/// <summary>
/// Context information for navigation building.
/// </summary>
public class DashboardNavigationContext
{
    /// <summary>
    /// Gets the current dashboard area type.
    /// </summary>
    public required DashboardArea Area { get; init; }

    /// <summary>
    /// Gets the navigation position (left or right drawer).
    /// </summary>
    public NavigationPosition Position { get; init; } = NavigationPosition.Left;

    /// <summary>
    /// Gets the current user's organization ID if available.
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// Gets the current user's project ID if available.
    /// </summary>
    public Guid? ProjectId { get; init; }
}

/// <summary>
/// Dashboard area types.
/// </summary>
public enum DashboardArea
{
    /// <summary>Main manager panel for regular users.</summary>
    Manager,
    /// <summary>Management panel for team admins.</summary>
    Management,
    /// <summary>Diagnostic panel for developers.</summary>
    Diagnostic,
    /// <summary>Public pages without authentication.</summary>
    Public
}

/// <summary>
/// Navigation position in the dashboard layout.
/// </summary>
public enum NavigationPosition
{
    /// <summary>Left side navigation drawer.</summary>
    Left,
    /// <summary>Right side navigation drawer.</summary>
    Right
}

/// <summary>
/// Drawer anchor position.
/// </summary>
public enum DrawerAnchor
{
    /// <summary>Start of the layout (left in LTR).</summary>
    Start,
    /// <summary>End of the layout (right in LTR).</summary>
    End,
    /// <summary>Top of the layout.</summary>
    Top,
    /// <summary>Bottom of the layout.</summary>
    Bottom
}

/// <summary>
/// Toolbar position in the appbar.
/// </summary>
public enum ToolbarPosition
{
    /// <summary>Left side of the appbar.</summary>
    Left,
    /// <summary>Right side of the appbar.</summary>
    Right
}
