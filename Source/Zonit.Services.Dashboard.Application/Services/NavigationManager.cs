using System.Collections.Concurrent;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application.Services;

/// <summary>
/// Default implementation of <see cref="INavigationManager"/>.
/// Thread-safe navigation item registration and retrieval.
/// </summary>
internal sealed class NavigationManager : INavigationManager
{
    private readonly ConcurrentDictionary<(DashboardArea, NavigationPosition), ConcurrentBag<NavigationItem>> _items = new();

    /// <inheritdoc />
    public event Action? OnNavigationChanged;

    /// <inheritdoc />
    public void Register(DashboardArea area, NavigationItem item, NavigationPosition position = NavigationPosition.Left)
    {
        ArgumentNullException.ThrowIfNull(item);

        var bag = _items.GetOrAdd((area, position), _ => new ConcurrentBag<NavigationItem>());
        bag.Add(item);
    }

    /// <inheritdoc />
    public void Register(DashboardArea area, IEnumerable<NavigationItem> items, NavigationPosition position = NavigationPosition.Left)
    {
        ArgumentNullException.ThrowIfNull(items);

        var bag = _items.GetOrAdd((area, position), _ => new ConcurrentBag<NavigationItem>());
        foreach (var item in items)
        {
            bag.Add(item);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<NavigationItem> GetNavigation(DashboardNavigationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return GetItems(context.Area, context.Position);
    }

    /// <inheritdoc />
    public IReadOnlyList<NavigationItem> GetItems(DashboardArea area, NavigationPosition position = NavigationPosition.Left)
    {
        if (!_items.TryGetValue((area, position), out var bag))
        {
            return [];
        }

        return bag
            .OrderBy(x => x.Order)
            .ToList();
    }

    /// <inheritdoc />
    public void Clear()
    {
        _items.Clear();
        NotifyChanged();
    }

    /// <inheritdoc />
    public void Clear(DashboardArea area)
    {
        foreach (var position in Enum.GetValues<NavigationPosition>())
        {
            _items.TryRemove((area, position), out _);
        }
        NotifyChanged();
    }

    /// <inheritdoc />
    public void NotifyChanged()
    {
        OnNavigationChanged?.Invoke();
    }
}
