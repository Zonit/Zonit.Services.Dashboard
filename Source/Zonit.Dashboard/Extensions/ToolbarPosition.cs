namespace Zonit.Dashboard.Extensions;

/// <summary>Where in the dashboard appbar a toolbar extension renders.</summary>
public enum ToolbarPosition
{
    /// <summary>Left of the brand / title region.</summary>
    Start,

    /// <summary>Centred between Start and End — rarely used; reserved for search bars / breadcrumb-style nav.</summary>
    Center,

    /// <summary>Right of the appbar — the conventional spot for user actions (notifications, profile menu, drawer toggles).</summary>
    End,
}
