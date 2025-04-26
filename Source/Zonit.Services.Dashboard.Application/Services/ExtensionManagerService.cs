using System.Collections.Concurrent;

namespace Zonit.Services.Dashboard.Application.Services;

internal class ExtensionManagerService : IExtensionManager
{
    private readonly ConcurrentDictionary<string, bool> _drawerStates = new();
    private readonly ConcurrentDictionary<string, DrawerImplementation> _drawers = new();

    public IDrawer Drawer(string extension) // Zwraca istniejącą instancję lub tworzy nową, jeśli nie istnieje dla danego rozszerzenia
        => _drawers.GetOrAdd(extension, key => new DrawerImplementation(_drawerStates, key));
    
    private class DrawerImplementation(ConcurrentDictionary<string, bool> states, string extension) : IDrawer
    {
        private readonly ConcurrentDictionary<string, bool> _states = states;
        private readonly string _extension = extension;

        public event Action? OnChange;

        public bool IsOpen => _states.GetOrAdd(_extension, false);

        public void Open()
        {
            _states[_extension] = true;
            StateChanged();
        }

        public void Close()
        {
            _states[_extension] = false;
            StateChanged();
        }

        public bool Toggle()
        {
            var newState = _states.AddOrUpdate(_extension, true, (_, currentValue) => !currentValue);
            StateChanged();
            return newState;
        }

        public void Set(bool open)
        {
            _states[_extension] = open;
            StateChanged();
        }

        public void StateChanged()
            => OnChange?.Invoke();
    }
}
