namespace Zonit.Dashboard.Extensions.Services;

/// <summary>
/// Default <see cref="IExtensionRegistry"/>. Caches the filtered + sorted lists
/// on first access for the scope's lifetime — registrations don't change at
/// runtime (the underlying singleton lists are immutable after DI build).
/// </summary>
internal sealed class ExtensionRegistry : IExtensionRegistry
{
    private readonly IDashboardCurrentSite _site;
    private readonly IReadOnlyList<IDrawerExtension> _allDrawers;
    private readonly IReadOnlyList<IToolbarExtension> _allToolbars;

    private IReadOnlyList<IDrawerExtension>? _filteredDrawers;
    private IReadOnlyList<IToolbarExtension>? _filteredToolbars;

    public ExtensionRegistry(
        IDashboardCurrentSite site,
        IEnumerable<IDrawerExtension> drawers,
        IEnumerable<IToolbarExtension> toolbars)
    {
        _site = site;
        _allDrawers = drawers.ToArray();
        _allToolbars = toolbars.ToArray();
    }

    public IReadOnlyList<IDrawerExtension> GetDrawerExtensions()
        => _filteredDrawers ??= Filter(_allDrawers);

    public IReadOnlyList<IToolbarExtension> GetToolbarExtensions()
        => _filteredToolbars ??= Filter(_allToolbars);

    public IDrawerExtension? GetDrawer(string id)
        => GetDrawerExtensions().FirstOrDefault(
            d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));

    public IToolbarExtension? GetToolbar(string id)
        => GetToolbarExtensions().FirstOrDefault(
            t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

    private List<T> Filter<T>(IReadOnlyList<T> all) where T : IDashboardExtension
    {
        // Build the whitelist as a HashSet for O(1) lookup. When the whitelist is
        // null we treat every extension as in-set; when it's empty we treat NONE as
        // in-set (deliberate — letting an empty list be "show all" would surprise
        // a host that intentionally locked everything off for this mount).
        HashSet<string>? whitelist = _site.ExtensionsWhitelist is { } ids
            ? new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase)
            : null;

        var result = new List<T>(all.Count);
        foreach (var ext in all)
        {
            if (!ext.IsEnabled) continue;
            if (whitelist is not null && !whitelist.Contains(ext.Id)) continue;
            result.Add(ext);
        }

        result.Sort((a, b) => a.Order.CompareTo(b.Order));
        return result;
    }
}
