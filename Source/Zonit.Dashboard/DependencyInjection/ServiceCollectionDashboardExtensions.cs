using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Translations;
using Zonit.Dashboard;
using Zonit.Dashboard.Components.Layouts;
using Zonit.Dashboard.Extensions;
using Zonit.Dashboard.Extensions.Builtin;
using Zonit.Dashboard.Extensions.Services;
using Zonit.Dashboard.Services;
using Zonit.Dashboard.Themes;
using Zonit.Dashboard.Themes.Builtin;
using Zonit.Dashboard.Themes.Services;
using Zonit.Extensions.Website;

namespace Zonit.Extensions;

/// <summary>
/// Services-time entry point for <c>Zonit.Dashboard</c>. Pairs with the
/// middleware-time <c>app.UseDashboard(...)</c>.
/// </summary>
public static class ServiceCollectionDashboardExtensions
{
    /// <summary>
    /// Registers the dashboard's process-wide services. Idempotent — safe to call
    /// more than once; everything inside uses <c>TryAdd*</c>.
    /// </summary>
    /// <remarks>
    /// <para><b>Why no <c>Action&lt;...&gt;</c> here?</b> All <em>per-mount</em>
    /// configuration (Permission, Layout knobs, ExtensionsWhitelist, CustomSnippet,
    /// the mirrored <c>SiteOptions</c> properties) is passed to
    /// <c>app.UseDashboard(directory, o =&gt; …)</c>, mirroring 1:1 the
    /// <c>app.UseWebsite&lt;TApp&gt;(directory, o =&gt; …)</c> shape. <c>AddDashboard()</c>
    /// only wires the things that are truly global (DI registrations, MudBlazor host
    /// services, layout-key bindings) — there's nothing per-mount left for it to take.</para>
    ///
    /// <para>What gets wired:</para>
    /// <list type="bullet">
    ///   <item><c>AddWebsite()</c> + <c>AddArea&lt;DashboardArea&gt;</c> — idempotent;
    ///         ensures the Website host is up so the dashboard can ride on it.</item>
    ///   <item><see cref="IDashboardCurrentSite"/> as scoped — populated by the
    ///         per-mount middleware that <c>UseDashboard()</c> installs.</item>
    ///   <item>MudBlazor services + translations (the dashboard's UI library).</item>
    ///   <item><c>"Dashboard.Main"</c> in <c>ILayoutRegistry</c> so plug-in pages can
    ///         <c>[LayoutKey("Dashboard.Main")]</c> without referencing this assembly.</item>
    ///   <item><c>"Zonit.Minimal"</c> overwritten with the dashboard-branded minimal
    ///         layout so framework-default error pages match the dashboard look.</item>
    /// </list>
    /// </remarks>
    [RequiresUnreferencedCode("Calls AddWebsite: Razor Components and Antiforgery use reflection, "
        + "and components from area assemblies are discovered dynamically.")]
    [RequiresDynamicCode("Calls AddWebsite: Razor Components and Antiforgery may emit dynamic code at runtime.")]
    public static IServiceCollection AddDashboard(this IServiceCollection services)
    {
        // Ensure the Website framework's infrastructure is wired. Idempotent —
        // AddWebsite uses TryAdd* throughout, so calling it after the consumer's
        // own AddWebsite(o => ...) is a no-op on the singletons.
        services.AddWebsite();

        // Register DashboardArea directly against the singleton WebsiteAreaRegistry.
        //
        // Why not `services.AddWebsite(o => o.AddArea<DashboardArea>())`?  AddWebsite
        // calls `services.TryAddSingleton(new WebsiteAreaRegistry())` — the second
        // call's freshly-allocated registry is discarded by TryAddSingleton, but the
        // local WebsiteOptions was already wired to that doomed registry, so any
        // AddArea<T>() inside its configure callback registers on an orphan and
        // the subsequent `Resolve<T>()` at middleware time throws.
        //
        // Direct registration on the singleton sidesteps the trap entirely and works
        // regardless of which order the consumer calls AddWebsite / AddDashboard.
        var registryDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(WebsiteAreaRegistry))
            ?? throw new InvalidOperationException(
                "Zonit.Dashboard: WebsiteAreaRegistry singleton not found after AddWebsite(). " +
                "This indicates Zonit.Extensions.Website is broken — file a bug.");

        if (registryDescriptor.ImplementationInstance is not WebsiteAreaRegistry registry)
            throw new InvalidOperationException(
                "Zonit.Dashboard: WebsiteAreaRegistry is registered without an instance — " +
                "expected AddWebsite() to use AddSingleton(new WebsiteAreaRegistry()).");

        var area = registry.Register(new DashboardArea());
        if (area is IWebsiteServices svc)
            svc.ConfigureServices(services);

        services.TryAddScoped<IDashboardCurrentSite, DashboardCurrentSite>();

        // Singleton route-assemblies map keyed by mount path. Survives the HTTP-scope
        // → SignalR-circuit-scope transition that the scoped ICurrentSite/IDashboardCurrentSite
        // do NOT (the per-Site branch middleware only fires for HTTP requests).
        // Populated per mount by UseDashboard() at startup, read by Routes.razor's
        // <Router AdditionalAssemblies>.
        services.TryAddSingleton<DashboardMountRegistry>();

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 300;
            config.SnackbarConfiguration.ShowTransitionDuration = 400;
        });
        services.AddMudTranslations();

        // Replace the default queue-based IToastProvider with a MudBlazor snackbar
        // adapter — the dashboard renders <MudSnackbarProvider/> in its layout, so
        // toasts must reach MudBlazor rather than sit in an unread queue. Replace
        // (not TryAdd) because Website's AddToastsExtension already registered the
        // default with TryAddScoped and we want to override it.
        services.RemoveAll<IToastProvider>();
        services.AddScoped<IToastProvider, ToastService>();

        services.AddWebsiteLayout<DashboardMainLayout>("Dashboard.Main");
        services.AddWebsiteLayout<DashboardMinimalLayout>("Zonit.Minimal");
        services.AddWebsiteLayout<DashboardMinimalLayout>("Dashboard.Minimal");
        services.AddWebsiteLayout<DashboardEmptyLayout>("Dashboard.Empty");

        // Theme system — IThemeManager + 3 built-ins. The first registration
        // (Default) becomes the fallback when no cookie is set. Hosts can append
        // their own with services.AddDashboardTheme<TheirTheme>() and they will
        // show up in the selector after the built-ins.
        services.TryAddScoped<IThemeManager, ThemeManager>();
        services.AddDashboardTheme<DefaultDashboardTheme>();
        services.AddDashboardTheme<OceanDashboardTheme>();
        services.AddDashboardTheme<ForestDashboardTheme>();

        // Extension system — registry + per-circuit drawer open/close state +
        // a single built-in extension (theme selector). The selector proves the
        // pipeline end-to-end on a fresh mount; consumers can disable it via the
        // per-mount DashboardSiteOptions.ExtensionsWhitelist if they want a
        // completely empty appbar.
        services.TryAddScoped<IExtensionRegistry, ExtensionRegistry>();
        services.TryAddScoped<IExtensionDrawerStates, ExtensionDrawerStates>();
        services.AddDrawerExtension<ThemeSelectorDrawerExtension>();
        services.AddDrawerExtension<CultureSwitcherDrawerExtension>();
        services.AddDrawerExtension<WorkspaceSwitcherDrawerExtension>();
        services.AddDrawerExtension<ProjectSwitcherDrawerExtension>();
        services.AddToolbarExtension<UserProfileToolbarExtension>();

        return services;
    }
}
