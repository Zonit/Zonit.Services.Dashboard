using System.Collections.Concurrent;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application.Services;

/// <summary>
/// Default implementation of <see cref="IExtensionDrawerManager"/>.
/// Manages drawer state for dashboard extensions.
/// </summary>
internal sealed class ExtensionDrawerManager : IExtensionDrawerManager
{
    private readonly ConcurrentDictionary<string, DrawerInstance> _drawers = new();

    /// <inheritdoc />
    public IExtensionDrawer GetDrawer(string extensionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extensionId);
        return _drawers.GetOrAdd(extensionId, _ => new DrawerInstance());
    }

    /// <summary>
    /// Internal drawer implementation.
    /// </summary>
    private sealed class DrawerInstance : IExtensionDrawer
    {
        private volatile bool _isOpen;

        /// <inheritdoc />
        public bool IsOpen => _isOpen;

        /// <inheritdoc />
        public event Action? OnChange;

        /// <inheritdoc />
        public void Open()
        {
            if (_isOpen) return;
            _isOpen = true;
            OnChange?.Invoke();
        }

        /// <inheritdoc />
        public void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;
            OnChange?.Invoke();
        }

        /// <inheritdoc />
        public bool Toggle()
        {
            _isOpen = !_isOpen;
            OnChange?.Invoke();
            return _isOpen;
        }

        /// <inheritdoc />
        public void SetOpen(bool open)
        {
            if (_isOpen == open) return;
            _isOpen = open;
            OnChange?.Invoke();
        }
    }
}
