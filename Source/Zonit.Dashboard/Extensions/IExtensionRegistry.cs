namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Per-mount view of registered drawer / toolbar extensions, filtered by the
/// active <see cref="DashboardSiteOptions.ExtensionsWhitelist"/>. The dashboard's
/// <c>DashboardMainLayout</c> queries this instead of the raw
/// <c>IEnumerable&lt;IDrawerExtension&gt;</c> so multi-mount scenarios
/// (e.g. <c>/admin</c> vs <c>/operator</c> with different feature sets) just work.
/// </summary>
/// <remarks>
/// <para>Lifetime: scoped — reads <see cref="IDashboardCurrentSite"/> at resolve
/// time and caches the filtered + sorted lists for the duration of the request.</para>
///
/// <para><b>Filter precedence</b> (extension survives iff all three pass):</para>
/// <list type="number">
///   <item><see cref="IDashboardExtension.IsEnabled"/> == <see langword="true"/>.</item>
///   <item><see cref="IDashboardCurrentSite.ExtensionsWhitelist"/> is <see langword="null"/>
///         <em>or</em> contains the extension's <see cref="IDashboardExtension.Id"/>
///         (case-insensitive).</item>
///   <item>Implements the requested slot interface
///         (<see cref="IDrawerExtension"/> / <see cref="IToolbarExtension"/>).</item>
/// </list>
/// </remarks>
public interface IExtensionRegistry
{
    /// <summary>All drawer extensions enabled for the current mount, sorted by <see cref="IDashboardExtension.Order"/>.</summary>
    IReadOnlyList<IDrawerExtension> GetDrawerExtensions();

    /// <summary>All toolbar extensions enabled for the current mount, sorted by <see cref="IDashboardExtension.Order"/>.</summary>
    IReadOnlyList<IToolbarExtension> GetToolbarExtensions();

    /// <summary>Single drawer extension by id, or <see langword="null"/> if it's not registered / not enabled for the current mount.</summary>
    IDrawerExtension? GetDrawer(string id);

    /// <summary>Single toolbar extension by id, or <see langword="null"/> if it's not registered / not enabled for the current mount.</summary>
    IToolbarExtension? GetToolbar(string id);
}
