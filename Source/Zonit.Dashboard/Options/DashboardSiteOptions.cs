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
    /// <remarks>
    /// <para><b>This is the ONLY way host <c>&lt;head&gt;</c> content reaches a dashboard
    /// mount.</b> <c>UseDashboard</c> renders the package's own <c>DashboardApp</c>, which emits
    /// the whole document — your application's <c>App.razor</c> is not involved. Web fonts,
    /// icon-font stylesheets, a Tailwind CDN tag, <c>app.css</c>, analytics: none of it carries
    /// over unless it is repeated here. The one exception is the Blazor scoped-CSS bundle
    /// (<c>{ApplicationName}.styles.css</c>), which <c>DashboardApp</c> links on its own —
    /// losing that one silently disables CSS isolation across the whole application, so it is
    /// not left to memory.</para>
    /// </remarks>
    public string? CustomSnippet { get; set; }

    /// <summary>
    /// Per-mount theme overrides layered on top of <c>Tenant.Settings.Theme</c>.
    /// Set individual color slots (Primary/Secondary/Accent/Neutral/Surface/Content)
    /// to make this mount visually distinct without changing the tenant brand.
    /// </summary>
    public DashboardThemeOverrides Theme { get; } = new();

    /// <summary>
    /// Where this mount's identity screens live and, critically, which host-owned URLs they post
    /// to. The dashboard renders identity screens; it never creates or destroys a session — see
    /// <see cref="DashboardIdentityOptions"/> for what went wrong when it did.
    /// </summary>
    public DashboardIdentityOptions Identity { get; } = new();

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

        // A dashboard is never a search result. The framework already derives that from
        // SiteOptions.Permission, but a panel mounted without one — an internal tool behind a
        // VPN, a staging admin, anything still being wired up — would be indexable by default,
        // and the failure is silent until panel URLs turn up in a search. Stated outright here
        // so it cannot be forgotten; a consumer who genuinely wants a public dashboard sets
        // Indexable = true in their configure lambda, which runs after this.
        Indexable = false;

        // No culture in the path. A panel is behind a login, is never indexed, and gains nothing
        // from a language segment except longer bookmarks. This is also the default, and it is
        // repeated here so that a future change to the framework default cannot quietly start
        // rewriting dashboard URLs.
        Cultures.Strategy = CultureUrlStrategy.None;

        // The framework's client-side appearance script is off here. It stamps a data-theme
        // attribute and keeps the HTML identical for every visitor, which is what makes a public
        // site cacheable — but MudThemeProvider resolves its palette on the SERVER, so the
        // dashboard needs the opposite handshake: tell the server the visitor's system
        // preference, via a cookie, before the first render. DashboardHead does that. Running
        // both would leave two scripts fighting over the same root element.
        Appearance.Enabled = false;

        // Document contents, declared rather than hand-written. AppBase emits them in the right
        // places and resolves each through the static-asset manifest for cache-busting.
        Document
            // Relative paths intentionally — they resolve against <base href> into
            // /dashboard/_content/... and are served by the per-branch MapStaticAssets().
            // Absolute /_content/... would not work: blazor.web.js hard-codes its negotiate URL
            // as a relative path, so the SignalR hub lives under the mount prefix anyway and a
            // mixed scheme buys nothing.
            .AddStylesheet("https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap")
            .AddStylesheet("_content/MudBlazor/MudBlazor.min.css")
            // Dashboard chrome AFTER MudBlazor on purpose: its rules read the --mud-palette-*
            // custom properties MudThemeProvider writes, and the later sheet wins on equal
            // specificity — so a consumer can still override with their own stylesheet, which
            // AppBase emits after both as the scoped-CSS bundle.
            .AddStylesheet("_content/ui/dashboard.css")
            .AddScript("_content/MudBlazor/MudBlazor.min.js")
            .AddHeadComponent<Components.DashboardHead>()
            // Branded reconnect modal. Blazor Server's framework JS toggles the
            // #components-reconnect-modal classes; this component only supplies the DOM and CSS
            // so the indicator matches the dashboard chrome.
            .AddBodyEndComponent<Components.Connection>();

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
        mounts.Register(Directory, Layout, ExtensionsWhitelist, CustomSnippet, Theme, Identity);

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
                current.Set(Layout, ExtensionsWhitelist, CustomSnippet, Theme, Identity);
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

    /// <summary>
    /// Render the right-hand drawer extensions (theme, workspace, project, culture, and anything
    /// a host registers). Turning this off also removes the app-bar buttons that open them, since
    /// their only purpose is to open a drawer that would no longer exist.
    /// </summary>
    /// <remarks>
    /// Declared and documented from the start, and read by nothing — setting it to
    /// <see langword="false"/> changed nothing on screen. It is wired now.
    /// </remarks>
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

    /// <summary>
    /// Width in pixels for right-hand drawer extensions that do not state their own.
    /// </summary>
    /// <remarks>
    /// 320 rather than the 280 this said before, because 320 is the width the panels were
    /// actually rendering at — <see cref="Zonit.Dashboard.Extensions.IDrawerExtension.Width"/> hard-coded it and this option
    /// was read by nothing. Matching the real value means wiring the option up changes no pixels
    /// for a host that never set it, which is the only honest way to fix a setting that has
    /// silently done nothing.
    /// </remarks>
    public int RightDrawerWidth { get; set; } = 320;

    /// <summary>Right rail width (px). The rail itself is narrow because every
    /// row is a label + icon; the actual switcher UI opens in a separate drawer.</summary>
    public int RightRailWidth { get; set; } = 220;

    /// <summary>Render breadcrumbs above the page body.</summary>
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Render the page's own <c>PageMeta.Title</c> as the <c>&lt;h1&gt;</c> above its body.
    /// </summary>
    /// <remarks>
    /// <para>The title is already declared — it composes the document title and the Open Graph
    /// tags — so a page that also writes its own heading is stating the same string twice, and the
    /// two drift the first time someone edits one of them. Turning this on lets a page delete the
    /// heading markup entirely, the same way breadcrumbs already work here.</para>
    ///
    /// <para><b>On by default — and it is a breaking change for a dashboard that already renders
    /// its own headings</b>, which will now show two. That is deliberate: the correct end state is
    /// pages that state a title and nothing else, and a default of <see langword="false"/> would
    /// leave every project one forgotten setting away from never getting there. Set it to
    /// <see langword="false"/> to keep the old behaviour while the markup is removed.</para>
    /// </remarks>
    public bool ShowPageTitle { get; set; } = true;

    /// <summary>
    /// Honour <c>PageWidth</c> when sizing the content container.
    /// </summary>
    /// <remarks>
    /// <para>The mapping is deliberately conservative: <c>Content</c> — what a page gets when it
    /// says nothing — stays <c>MaxWidth.False</c>, exactly what this layout did before, so nothing
    /// existing moves. Only a page that asks for something else gets something else.</para>
    ///
    /// <para><c>Content</c> and <c>Wide</c> resolve to the same width here, and that is honest
    /// rather than an oversight: a dashboard's ordinary page <em>is</em> full-bleed, so the
    /// distinction the enum draws for a content site has nothing to separate here. The value of the
    /// mapping is at the other end — a settings form that says <c>Narrow</c> and stops being a
    /// single input stretched across an ultrawide monitor.</para>
    /// </remarks>
    public bool HonourPageWidth { get; set; } = true;

    /// <summary>Render top progress bar while long-running tasks are executing.</summary>
    public bool ShowProgressBar { get; set; } = true;

    /// <summary>Enable touch-swipe gestures to open/close drawers on mobile.</summary>
    public bool EnableSwipeGestures { get; set; } = true;

    /// <summary>Appbar (top bar) elevation; valid range 0-24 per MudBlazor.</summary>
    public int AppbarElevation { get; set; } = 1;
}
