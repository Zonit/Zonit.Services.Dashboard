using MudBlazor;

namespace Zonit.Dashboard.Themes;

/// <summary>
/// One pre-built dashboard theme exposed in the theme selector. Wraps a fully-built
/// MudBlazor <see cref="MudTheme"/> with discovery metadata (<see cref="Id"/>,
/// <see cref="Name"/>, <see cref="Description"/>) so the selector UI can list it
/// without instantiating a separate registry.
/// </summary>
/// <remarks>
/// <para>Multiple themes register as singletons under <see cref="IDashboardTheme"/> —
/// <see cref="IThemeManager"/> uses <c>IEnumerable&lt;IDashboardTheme&gt;</c> to
/// discover all registered. Hosts add their own via
/// <c>services.AddDashboardTheme&lt;HostTheme&gt;()</c>; the dashboard ships three
/// built-ins (<c>Default</c> / <c>Ocean</c> / <c>Forest</c>) so the picker never
/// renders empty.</para>
///
/// <para><b>Brand colors vs theme colors.</b> The built-in <c>Default</c> theme
/// reads <c>Tenant.Settings.Theme</c> (PrimaryColor / SecondaryColor / AccentColor
/// / etc.) so the per-tenant brand defaults flow through automatically. Custom
/// themes (Ocean / Forest / host-specific) hard-code their own palette by design
/// — they exist precisely to give the user an aesthetic choice that overrides the
/// tenant brand. The theme selector lets the user pick which behavior they want.</para>
/// </remarks>
public interface IDashboardTheme
{
    /// <summary>Stable string identifier used in cookies and theme-selector URLs. Lowercase, no spaces.</summary>
    string Id { get; }

    /// <summary>Human-readable name displayed in the theme selector.</summary>
    string Name { get; }

    /// <summary>One-line description for the theme selector tooltip / subtitle.</summary>
    string Description { get; }

    /// <summary>
    /// Fully-built MudBlazor theme. Implementations may compute this lazily (once per
    /// instance) — themes are singletons, so the cost is paid at most once per process.
    /// </summary>
    MudTheme MudTheme { get; }
}
