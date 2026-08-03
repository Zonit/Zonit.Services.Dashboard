namespace Zonit.Dashboard.Themes;

/// <summary>
/// Per-circuit dashboard theme state. Tracks the user's selected
/// <see cref="IDashboardTheme"/> (<see cref="Current"/>) and light / dark / auto
/// preference (<see cref="Mode"/>) and notifies subscribers when either changes.
/// </summary>
/// <remarks>
/// <para>Scoped service. Each Blazor Server circuit has its own
/// <see cref="IThemeManager"/>; the selection is persisted to a cookie
/// (<c>zonit.dashboard.theme</c>) so a new circuit for the same browser starts
/// where the previous one left off.</para>
///
/// <para><b>Hydration timing.</b> The initial render uses the cookie value if the
/// request scope already had it (server-side prerender); the interactive circuit
/// later calls <see cref="HydrateAsync"/> from the dashboard layout's
/// <c>OnAfterRenderAsync(firstRender: true)</c> to bridge the request → circuit
/// scope split that Blazor Server cookies require (see <c>ICookieProvider.RefreshAsync</c>).</para>
///
/// <para><b>Mode resolution.</b> <see cref="Mode"/> is the user's <em>preference</em>
/// — Auto / Light / Dark. <see cref="IsDark"/> is the <em>resolved</em> boolean
/// MudBlazor needs for its <c>IsDarkMode</c> parameter: for <see cref="ThemeMode.Auto"/>
/// it consults the browser's <c>prefers-color-scheme</c> (set via JS interop during
/// hydration); for explicit Light/Dark it ignores the browser.</para>
/// </remarks>
public interface IThemeManager
{
    /// <summary>Active theme. Never <see langword="null"/>; defaults to the first registered theme when no cookie is present.</summary>
    IDashboardTheme Current { get; }

    /// <summary>User's light / dark preference.</summary>
    ThemeMode Mode { get; }

    /// <summary>
    /// Resolved dark-mode flag for the current render — feed straight into
    /// <c>&lt;MudThemeProvider IsDarkMode="@ThemeManager.IsDark" /&gt;</c>. Combines
    /// <see cref="Mode"/> with the system <c>prefers-color-scheme</c> media query.
    /// </summary>
    bool IsDark { get; }

    /// <summary>All themes the host registered, sorted by registration order. Used by the theme selector.</summary>
    IReadOnlyList<IDashboardTheme> Available { get; }

    /// <summary>
    /// Pulls the persisted selection from the cookie (and the system color-scheme
    /// preference) into <see cref="Current"/> / <see cref="Mode"/> / <see cref="IsDark"/>.
    /// Idempotent — safe to call repeatedly; the dashboard layout invokes it once
    /// per circuit in <c>OnAfterRenderAsync(firstRender: true)</c>.
    /// </summary>
    Task HydrateAsync();

    /// <summary>
    /// Switches the active theme by <see cref="IDashboardTheme.Id"/>. Persists to
    /// cookie and raises <see cref="OnChange"/>. Unknown ids are ignored (warning
    /// logged) — the registry validates strict mode at startup if enabled.
    /// </summary>
    Task SetThemeAsync(string themeId);

    /// <summary>Switches the active mode. Persists to cookie and raises <see cref="OnChange"/>.</summary>
    Task SetModeAsync(ThemeMode mode);

    /// <summary>Raised whenever <see cref="Current"/> / <see cref="Mode"/> / <see cref="IsDark"/> change.</summary>
    event Action? OnChange;
}
