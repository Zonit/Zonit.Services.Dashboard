namespace Zonit.Dashboard.Extensions;

/// <summary>
/// Per-circuit open/close state for every registered drawer extension. The
/// dashboard layout reads <see cref="GetState"/> to render the drawer's
/// <c>Open</c> parameter; extension UIs (toolbar toggle buttons, in-content
/// "close" links) drive the same state through <see cref="IExtensionDrawerState"/>.
/// </summary>
/// <remarks>
/// <para>Lifetime: scoped (per circuit). Resets to closed on every new circuit —
/// drawer state is intentionally <em>not</em> persisted between sessions; users
/// expect a fresh dashboard to start with no panels open.</para>
///
/// <para><b>Single-active-drawer policy</b> (per anchor side). Opening a drawer
/// on a side automatically closes any other drawer on the same side. This matches
/// the way <c>MudDrawer</c>s overlay each other and avoids the visually broken
/// "two panels stacked on top of each other" state the legacy
/// <c>IExtensionDrawerManager</c> allowed.</para>
/// </remarks>
public interface IExtensionDrawerStates
{
    /// <summary>Returns the state object for the given extension id; null when the id is unknown / not enabled for this mount.</summary>
    IExtensionDrawerState? GetState(string extensionId);

    /// <summary>Closes every open drawer on the given anchor.</summary>
    void CloseAll(DrawerAnchor anchor);

    /// <summary>Raised whenever any drawer's open state changes.</summary>
    event Action? OnChange;
}

/// <summary>One drawer's open / close state — manipulated by the toolbar toggle and by the drawer body itself.</summary>
public interface IExtensionDrawerState
{
    /// <summary>The extension this state belongs to.</summary>
    string ExtensionId { get; }

    /// <summary><see langword="true"/> when the drawer is currently visible.</summary>
    bool IsOpen { get; }

    /// <summary>Opens the drawer. Closes any other drawer on the same anchor.</summary>
    void Open();

    /// <summary>Closes the drawer. No-op when already closed.</summary>
    void Close();

    /// <summary>Toggles the drawer. Returns the new state.</summary>
    bool Toggle();
}
