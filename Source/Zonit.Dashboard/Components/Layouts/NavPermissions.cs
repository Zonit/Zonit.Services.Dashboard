using Zonit.Extensions;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Components.Layouts;

/// <summary>
/// Decides which navigation nodes the current identity may see.
/// </summary>
/// <remarks>
/// <para><b>Why this lives here and not in the kernel.</b> <c>NavigationService.Get</c> filters by
/// Site only, and says so: <i>"Permission-based filtering is intentionally NOT done here — pass a
/// predicate-style filter in the UI layer if needed."</i> This is that UI layer. Doing it during
/// rendering also means nothing has to be copied: a hidden node is simply not emitted, where a
/// kernel-side filter would have to rebuild every <see cref="NavGroup"/> and <see cref="NavItem"/>
/// on the path to it — they are classes with <c>init</c> setters, so "filter" would mean a
/// hand-written copy of fourteen properties that silently drops data the day someone adds a
/// fifteenth.</para>
///
/// <para><b>What it fixes.</b> <see cref="NavGroup.Permission"/> is documented as <i>"Optional
/// permission required to see this group"</i>, and until now nothing anywhere honoured it —
/// measured: an anonymous visitor with no permissions at all saw every gated group and item in
/// the menu. That was never an access hole (the target mount still answers 401 through
/// <c>SiteOptions.Permission</c>) but a menu that offers doors the visitor cannot open is its own
/// kind of broken.</para>
///
/// <para><b>Empty permission means no requirement.</b> <c>Identity.HasPermission(default)</c>
/// returns <see langword="true"/>, so the overwhelmingly common case — a node that declares no
/// permission — passes without a special case here.</para>
/// </remarks>
internal static class NavPermissions
{
    /// <summary>Whether <paramref name="item"/> should be rendered for <paramref name="identity"/>.</summary>
    public static bool IsVisible(NavItem item, Identity identity)
        => identity.HasPermission(item.Permission);

    /// <summary>
    /// Whether <paramref name="group"/> should be rendered for <paramref name="identity"/>.
    /// </summary>
    /// <remarks>
    /// A group also disappears when its own permission passes but every child was filtered out:
    /// an expandable header that opens onto nothing is worse than no header, because it reads as
    /// a section that failed to load. A group that carries its own <see cref="NavGroup.Link"/>
    /// stays — it is a destination in its own right, not just a container.
    /// </remarks>
    public static bool IsVisible(NavGroup group, Identity identity)
    {
        if (!identity.HasPermission(group.Permission))
            return false;

        if (group.Link is not null)
            return true;

        var hasAnyChild =
            group.Children is { Count: > 0 } ||
            group.Groups is { Count: > 0 };

        if (!hasAnyChild)
            return true;   // a bare section caption — nothing to empty out

        if (group.Children is not null)
        {
            foreach (var item in group.Children)
                if (IsVisible(item, identity))
                    return true;
        }

        if (group.Groups is not null)
        {
            foreach (var sub in group.Groups)
                if (IsVisible(sub, identity))
                    return true;
        }

        return false;
    }
}
