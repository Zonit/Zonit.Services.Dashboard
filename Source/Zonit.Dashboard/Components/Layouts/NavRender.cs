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
    /// <summary>Whether any entry in a sibling list declares an icon.</summary>
    /// <remarks>
    /// Drives the reserved icon column. Only markup counts as an icon — a bare name is dropped
    /// by <c>NavIcon</c>, so treating it as present here would reserve a column that then stays
    /// empty for that row and misalign exactly the list this is meant to keep straight.
    /// </remarks>
    public static bool AnyIcon(IReadOnlyList<NavItem>? items)
    {
        if (items is null) return false;
        foreach (var item in items)
            if (IsMarkup(item.Icon)) return true;
        return false;
    }

    /// <inheritdoc cref="AnyIcon(IReadOnlyList{NavItem})"/>
    public static bool AnyIcon(IReadOnlyList<NavGroup>? groups)
    {
        if (groups is null) return false;
        foreach (var group in groups)
            if (IsMarkup(group.Icon)) return true;
        return false;
    }

    private static bool IsMarkup(string? icon)
        => icon is not null && icon.TrimStart() is { Length: > 0 } v && v[0] == '<';

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
