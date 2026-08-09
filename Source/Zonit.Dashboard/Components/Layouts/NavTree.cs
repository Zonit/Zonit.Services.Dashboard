using Microsoft.AspNetCore.Components;
using Zonit.Extensions;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Components.Layouts;

/// <summary>
/// Active-route arithmetic for the sidebar tree.
/// </summary>
/// <remarks>
/// <para>Blazor's own <c>NavLink</c> can decide whether <em>one</em> link is active, and that
/// is all the leaf rows need. A tree also has to answer a second question: <em>does anything
/// below this node match the current URL</em> — because a branch whose child is the open page
/// must render already-expanded. Without it a deep link (say <c>/docs/vo/exceptions</c>) lands
/// the visitor on a page whose entire ancestry is collapsed, so the sidebar shows no trace of
/// where they are and the deeper a tree grows the more useless it gets.</para>
///
/// <para>Matching is done on <em>segments</em>, not on <c>string.StartsWith</c>: "vo" must not
/// count as a prefix match for "voice". This is the same rule the permission grammar uses, and
/// getting it wrong here highlights the wrong branch rather than denying access, which is why
/// it is worth stating explicitly.</para>
/// </remarks>
internal static class NavTree
{
    /// <summary>
    /// Current location as a base-relative path with query, fragment and surrounding
    /// slashes removed — the form the <c>UrlPath</c>-derived hrefs are comparable to.
    /// </summary>
    /// <remarks>
    /// Base-relative, not absolute: nav hrefs go through <see cref="UrlPathRendering.ToHref"/>
    /// and are therefore relative to <c>&lt;base href&gt;</c>. Comparing them against an
    /// absolute path would never match on a non-root mount such as <c>/docs</c>.
    /// </remarks>
    public static string CurrentPath(NavigationManager navigation)
    {
        var relative = navigation.ToBaseRelativePath(navigation.Uri);

        var cut = relative.IndexOfAny(['?', '#']);
        if (cut >= 0) relative = relative[..cut];

        return relative.Trim('/');
    }

    /// <summary>Whether <paramref name="group"/> or anything beneath it points at <paramref name="current"/>.</summary>
    public static bool ContainsActive(NavGroup group, string current)
    {
        if (group.Link is not null && IsActive(group.Link.Url, match: false, current))
            return true;

        if (group.Children is not null)
        {
            foreach (var item in group.Children)
                if (ContainsActive(item, current))
                    return true;
        }

        if (group.Groups is not null)
        {
            foreach (var sub in group.Groups)
                if (ContainsActive(sub, current))
                    return true;
        }

        return false;
    }

    /// <summary>Whether <paramref name="item"/> or any descendant points at <paramref name="current"/>.</summary>
    public static bool ContainsActive(NavItem item, string current)
    {
        if (item.IsMatchable() && IsActive(item.Url, item.Match, current))
            return true;

        if (item.Children is not null)
        {
            foreach (var child in item.Children)
                if (ContainsActive(child, current))
                    return true;
        }

        return false;
    }

    /// <summary>
    /// Segment-aware URL comparison mirroring <c>NavLinkMatch.All</c> / <c>NavLinkMatch.Prefix</c>.
    /// </summary>
    private static bool IsActive(UrlPath url, bool match, string current)
    {
        var href = url.ToHref().Trim('/');
        if (href.Length == 0)
            return current.Length == 0;

        if (current.Equals(href, StringComparison.OrdinalIgnoreCase))
            return true;

        if (match)
            return false;

        // Prefix mode: only on a segment boundary. "vo" must not light up for "voice",
        // which a bare StartsWith would happily do.
        return current.Length > href.Length
            && current[href.Length] == '/'
            && current.StartsWith(href, StringComparison.OrdinalIgnoreCase);
    }
}
