using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Components.Layouts;

/// <summary>
/// Presentation mapping shared by <c>RenderNavGroup</c> and <c>RenderNavItem</c>.
/// </summary>
/// <remarks>
/// Lives here rather than duplicated in both components' <c>@code</c> blocks so a new
/// <see cref="NavBadgeColor"/> member cannot be handled in one renderer and silently fall
/// through to the default in the other.
/// </remarks>
internal static class NavRender
{
    public static string BadgeClass(NavBadgeColor color) => color switch
    {
        NavBadgeColor.Primary => "znav-badge-primary",
        NavBadgeColor.Secondary => "znav-badge-secondary",
        NavBadgeColor.Success => "znav-badge-success",
        NavBadgeColor.Warning => "znav-badge-warning",
        NavBadgeColor.Error => "znav-badge-error",
        NavBadgeColor.Info => "znav-badge-info",
        _ => string.Empty
    };
}
