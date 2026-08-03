using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zonit.Extensions;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard;

/// <summary>
/// Per-mount configuration for a single <c>app.UseDashboard(...)</c> call. Inherits
/// the full surface of <see cref="SiteOptions"/> (Permission / Mode / Hsts /
/// HttpsRedirection / Compression / Proxy / AntiForgery / SecurityHeaders /
/// ExceptionHandlerPath / AddArea / App / Use / MapEndpoints) and adds the dashboard-
/// specific UI extras (<see cref="Layout"/>, <see cref="ExtensionsWhitelist"/>,
/// <see cref="CustomSnippet"/>).
/// </summary>
/// <remarks>
/// <para>Constructed reflectively by <c>UseWebsite&lt;TApp, TOptions&gt;</c> — the
/// internal ctor matches the framework's reflective <c>Activator.CreateInstance</c>
/// shape (single <see cref="UrlPath"/> arg, any access modifier). After construction
/// the framework calls <c>AttachRegistry</c> on the base before the consumer's
/// configuration lambda runs, so <see cref="SiteOptions.AddArea{TArea}"/> is
/// immediately usable.</para>
///
/// <para><b>Dashboard-specific extras</b> (<see cref="Layout"/> / whitelist / snippet)
/// are surfaced at runtime via the scoped <see cref="IDashboardCurrentSite"/> service —
/// dashboard layouts read it instead of digging into <see cref="DashboardSiteOptions"/>
/// (which doesn't survive past <c>UseDashboard()</c>).</para>
/// </remarks>
public class DashboardSiteOptions : SiteOptions
{
    public DashboardSiteOptions()
    {
    }

    /// <summary>
    /// Layout-level UI knobs that are genuinely dashboard-specific (drawer widths,
    /// breadcrumb visibility, etc.) and do not belong in <c>Tenant.Settings</c>
    /// because they describe a single dashboard mount, not the tenant identity.
    /// </summary>
    public DashboardLayoutOptions Layout { get; } = new();

    /// <summary>
    /// Optional whitelist of extension IDs (drawer / toolbar extensions). When set,
    /// only extensions whose <c>Id</c> appears in this list are rendered;
    /// <see langword="null"/> = render every registered extension.
    /// </summary>
    public string[]? ExtensionsWhitelist { get; set; }

    /// <summary>
    /// Optional HTML/JS snippet injected into the <c>&lt;head&gt;</c> of every page
    /// rendered under this mount. For analytics, error trackers, custom scripts.
    /// </summary>
    public string? CustomSnippet { get; set; }

    /// <summary>
    /// Per-mount theme overrides layered on top of <c>Tenant.Settings.Theme</c>.
    /// Set individual color slots (Primary/Secondary/Accent/Neutral/Surface/Content)
    /// to make this mount visually distinct without changing the tenant brand.
    /// </summary>
    public DashboardThemeOverrides Theme { get; } = new();

    /// <summary>
    /// Pre-configure hook — runs BEFORE the consumer's <c>configure</c> lambda. Seeds
    /// the implicit <see cref="DashboardArea"/> so every dashboard mount always carries
    /// the dashboard chrome, regardless of the consumer's per-mount Area selection.
    /// </summary>
    /// <remarks>
    /// <para>Adding <see cref="DashboardArea"/> here (rather than inside <c>UseDashboard</c>)
    /// keeps the lifecycle symmetric: the consumer's lambda still sees the area list
    /// pre-seeded just as it would if the framework had implicit defaults — and a
    /// derived <c>UseDashboard</c>-style host could opt out by overriding this method
    /// and skipping <c>base.OnConfiguring(...)</c>.</para>
    /// </remarks>
    protected override void OnConfiguring(IServiceProvider services)
    {
        base.OnConfiguring(services);

        // Implicit dashboard chrome — always mounted first so consumer hooks may
        // augment but not displace it.
        AddArea<DashboardArea>();
    }

    /// <summary>
    /// Post-configure hook — runs AFTER the consumer's <c>configure</c> lambda. Snapshots
    /// the final Area list into the singleton <see cref="DashboardMountRegistry"/>
    /// (read by <c>Routes.razor</c> from the SignalR circuit scope where
    /// <see cref="ICurrentSite"/> is not populated) and wires the per-mount
    /// <see cref="IDashboardCurrentSite"/> hydrator.
    /// </summary>
    /// <remarks>
    /// <para><b>No explicit <c>MapBlazorHub</c> here.</b> The hub (and its companion
    /// "Blazor initializers" endpoint at <c>/_framework/blazor-initializers</c>) is
    /// wired automatically by <c>MapRazorComponents&lt;TApp&gt;().AddInteractiveServerRenderMode()</c>
    /// inside <c>BuildBranch</c>, so the hub endpoint already lives in this branch's
    /// endpoint route builder under the correct path-base. Calling <c>MapBlazorHub</c>
    /// a second time registers a duplicate "Blazor initializers" endpoint with the
    /// same route pattern, which the routing matcher reports as
    /// <c>AmbiguousMatchException</c> the moment <c>blazor.web.js</c> fetches its
    /// initializer list (see github.com/dotnet/aspnetcore/issues/51698). For .NET 9+
    /// — and therefore .NET 10 — explicit <c>MapBlazorHub</c> is redundant whenever
    /// <c>AddInteractive*RenderMode</c> is used.</para>
    /// </remarks>
    protected override void OnConfigured(IServiceProvider services)
    {
        base.OnConfigured(services);

        // Snapshot the FULL per-mount state into the singleton registry — Routes.razor
        // reads route assemblies back from a SignalR circuit scope (where the
        // per-Site branch middleware never fires), and IDashboardCurrentSite reads
        // Layout / ExtensionsWhitelist / CustomSnippet from the same circuit scope
        // for the very same reason. Storing it once at startup lets every consumer
        // self-hydrate without a per-page bridge component or a PersistentComponentState
        // round-trip — both of which would be required if the state were truly
        // per-request, which it isn't (mount config is static for the host's lifetime).
        // See DashboardMountRegistry remarks for the full rationale.
        var mounts = services.GetRequiredService<DashboardMountRegistry>();
        mounts.Register(Directory, Areas, Layout, ExtensionsWhitelist, CustomSnippet, Theme);

        // Late-pipeline middleware: stamp IDashboardCurrentSite for this branch
        // BEFORE the page is resolved, so layouts and components see the per-mount
        // state from their first SSR render. The interactive pass reads the same
        // values straight from the singleton registry (via DashboardCurrentSite's
        // self-hydration), so the values agree across the SSR → circuit transition
        // and the chrome no longer "flashes" empty after hydration.
        Use(branch =>
        {
            branch.Use(async (ctx, next) =>
            {
                var current = ctx.RequestServices.GetRequiredService<IDashboardCurrentSite>();
                current.Set(Layout, ExtensionsWhitelist, CustomSnippet, Theme);
                await next();
            });
        });
    }
}

/// <summary>Layout-level dashboard UI options. Defaults reproduce the legacy look.</summary>
public sealed class DashboardLayoutOptions
{
    /// <summary>Show the left navigation drawer.</summary>
    public bool ShowLeftDrawer { get; set; } = true;

    /// <summary>Show the right extension drawer.</summary>
    public bool ShowRightDrawer { get; set; } = true;

    /// <summary>
    /// Show the persistent right-side rail with inline switchers (user profile,
    /// theme, organization, project, culture). Mirrors the legacy
    /// <c>Zonit.Services.Dashboard</c> right drawer that hosted a single column of
    /// labelled icon buttons opening secondary drawers — discoverable in one
    /// glance, no hidden behaviour behind appbar icons.
    /// </summary>
    public bool ShowRightRail { get; set; } = true;

    /// <summary>Left navigation drawer width (px).</summary>
    public int LeftDrawerWidth { get; set; } = 240;

    /// <summary>Right extension drawer width (px).</summary>
    public int RightDrawerWidth { get; set; } = 280;

    /// <summary>Right rail width (px). The rail itself is narrow because every
    /// row is a label + icon; the actual switcher UI opens in a separate drawer.</summary>
    public int RightRailWidth { get; set; } = 220;

    /// <summary>Render breadcrumbs above the page body.</summary>
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>Render top progress bar while long-running tasks are executing.</summary>
    public bool ShowProgressBar { get; set; } = true;

    /// <summary>Enable touch-swipe gestures to open/close drawers on mobile.</summary>
    public bool EnableSwipeGestures { get; set; } = true;

    /// <summary>Appbar (top bar) elevation; valid range 0-24 per MudBlazor.</summary>
    public int AppbarElevation { get; set; } = 1;
}
