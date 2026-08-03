using System.Collections.Concurrent;

namespace Zonit.Dashboard.Extensions.Services;

/// <summary>
/// Default <see cref="IExtensionDrawerStates"/>. Lazily creates a state object
/// per known extension id on first access; enforces the single-active-drawer-
/// per-anchor policy documented on <see cref="IExtensionDrawerStates"/>.
/// </summary>
internal sealed class ExtensionDrawerStates : IExtensionDrawerStates
{
    private readonly IExtensionRegistry _registry;

    // ConcurrentDictionary because IExtensionDrawerState methods may be invoked
    // from interactive renders (UI thread) and async render passes simultaneously
    // in Blazor Server; race-free TryAdd / TryGetValue covers it without a lock.
    private readonly ConcurrentDictionary<string, DrawerState> _states =
        new(StringComparer.OrdinalIgnoreCase);

    public ExtensionDrawerStates(IExtensionRegistry registry)
    {
        _registry = registry;
    }

    public event Action? OnChange;

    public IExtensionDrawerState? GetState(string extensionId)
    {
        var ext = _registry.GetDrawer(extensionId);
        if (ext is null) return null;

        return _states.GetOrAdd(ext.Id, id => new DrawerState(id, ext.Anchor, this));
    }

    public void CloseAll(DrawerAnchor anchor)
    {
        var changed = false;
        foreach (var s in _states.Values)
        {
            if (s.Anchor == anchor && s.IsOpen)
            {
                s.SetOpenInternal(false);
                changed = true;
            }
        }
        if (changed) OnChange?.Invoke();
    }

    internal void NotifyChanged() => OnChange?.Invoke();

    private sealed class DrawerState : IExtensionDrawerState
    {
        private readonly ExtensionDrawerStates _owner;

        public DrawerState(string id, DrawerAnchor anchor, ExtensionDrawerStates owner)
        {
            ExtensionId = id;
            Anchor = anchor;
            _owner = owner;
        }

        public string ExtensionId { get; }
        public DrawerAnchor Anchor { get; }
        public bool IsOpen { get; private set; }

        public void Open()
        {
            if (IsOpen) return;

            // Single-active policy: close every other drawer on this side first.
            foreach (var s in _owner._states.Values)
                if (s.Anchor == Anchor && s.IsOpen && !ReferenceEquals(s, this))
                    s.IsOpen = false;

            IsOpen = true;
            _owner.NotifyChanged();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            _owner.NotifyChanged();
        }

        public bool Toggle()
        {
            if (IsOpen) Close();
            else Open();
            return IsOpen;
        }

        internal void SetOpenInternal(bool value) => IsOpen = value;
    }
}
