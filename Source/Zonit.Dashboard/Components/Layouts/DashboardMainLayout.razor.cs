using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using Zonit.Dashboard.Extensions;
using Zonit.Dashboard.Themes;
using Zonit.Extensions.Cultures;
using Zonit.Extensions.Tenants;
using Zonit.Extensions.Website;

namespace Zonit.Dashboard.Components.Layouts;

/// <summary>
/// Code-behind for <c>DashboardMainLayout.razor</c>. Holds DI injection points,
/// state-change subscriptions (theme / drawer / breadcrumb / nav / tenant), drawer
/// toggle handlers, and the small projection helpers the markup queries
/// (<see cref="ToolbarStartExtensions"/>, <see cref="NavGroups"/>, etc.).
/// </summary>
/// <remarks>
/// <para><b>Subscription bookkeeping.</b> Every reactive source the layout renders
/// from gets a hook installed in <see cref="OnInitialized"/> and torn down in
/// <c>Dispose</c> — the layout's lifetime is the circuit's, so leaking these
/// would leak the whole layout instance.</para>
///
/// <para><b>Hydration.</b> <see cref="IThemeManager.HydrateAsync"/> runs once in
/// <c>OnAfterRenderAsync(firstRender: true)</c> — it needs JS (cookie refresh +
/// <c>prefers-color-scheme</c>) which isn't available during prerender.</para>
/// </remarks>
public sealed partial class DashboardMainLayout : LayoutComponentBase, IAsyncDisposable, IBrowserViewportObserver
{
    [Inject] private IThemeManager ThemeManager { get; set; } = default!;
    [Inject] private ITenantProvider Tenant { get; set; } = default!;
    [Inject] private ICurrentSite CurrentSite { get; set; } = default!;
    [Inject] private IDashboardCurrentSite Site { get; set; } = default!;
    // Renamed from "Navigation" because LayoutComponentBase already inherits an
    // [Inject] Navigation property of type Microsoft.AspNetCore.Components.NavigationManager.
    // Calling .Get(...) on that one returns a Task, which is the exact symptom that
    // led to renaming.
    [Inject] private INavigationProvider NavProvider { get; set; } = default!;
    [Inject] private IExtensionRegistry Extensions { get; set; } = default!;
    [Inject] private IExtensionDrawerStates DrawerStates { get; set; } = default!;
    [Inject] private IBreadcrumbsProvider BreadcrumbsProvider { get; set; } = default!;
    [Inject] private ICultureProvider Culture { get; set; } = default!;
    [Inject] private IBrowserViewportService BrowserViewport { get; set; } = default!;

    // Single piece of layout-local state — every other "open" flag belongs to a
    // service (drawer states in IExtensionDrawerStates, theme mode in IThemeManager).
    // The left drawer has no dedicated service because there's exactly one — it's
    // simpler to keep its open/close in the layout than to introduce a service for
    // a single boolean.
    private bool _leftDrawerOpen = true;

    // Persistent right rail (user/theme/org/project/culture inline switchers).
    // Default open on desktop; the viewport observer flips it to closed on
    // phones where there's no room for two persistent drawers side by side.
    private bool _rightRailOpen = true;
    private DrawerVariant _rightRailVariant = DrawerVariant.Persistent;

    // Responsive drawer state. Mirrors the legacy MainLayout.razor.cs breakpoint
    // logic: full-responsive drawer on desktop, mini-on-hover on mid-size, fully
    // collapsed on phones. Width is reported by IBrowserViewportObserver below.
    private DrawerVariant _leftDrawerVariant = DrawerVariant.Responsive;
    private bool _leftDrawerOpenMini;
    private int _viewportWidth = 1920;

    // RTL flag pulled from the active culture. Arabic / Hebrew / Persian flip the
    // entire MudBlazor layout via <MudRTLProvider>; everything else renders LTR.
    private bool IsRtl => Culture.Current.HasValue && IsRtlCulture(Culture.Current.ValueOrDefault);

    // Progress-bar visibility flag exposed to the layout markup so extensions can
    // turn it on/off without owning the layout. Read-only here — wired via
    // IExtensionRegistry once TaskManager is migrated (B9). For now stays false
    // so the line never paints unless something explicitly sets ProgressBarVisible
    // through a future IProgressSlot service. The DashboardLayoutOptions.ShowProgressBar
    // master switch gates the whole element regardless.
    internal bool ProgressBarVisible { get; private set; }

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    // ─── Projections the markup queries (computed per render; cheap LINQ over
    //     immutable singletons / scoped caches — no allocations in the hot path
    //     beyond the enumerator). ─────────────────────────────────────────────

    /// <summary>Toolbar extensions targeting the Start (left) slot.</summary>
    private IEnumerable<IToolbarExtension> ToolbarStartExtensions
        => Extensions.GetToolbarExtensions().Where(t => t.Position == ToolbarPosition.Start);

    /// <summary>Toolbar extensions targeting the Center slot.</summary>
    private IEnumerable<IToolbarExtension> ToolbarCenterExtensions
        => Extensions.GetToolbarExtensions().Where(t => t.Position == ToolbarPosition.Center);

    /// <summary>Toolbar extensions targeting the End (right) slot.</summary>
    private IEnumerable<IToolbarExtension> ToolbarEndExtensions
        => Extensions.GetToolbarExtensions().Where(t => t.Position == ToolbarPosition.End);

    /// <summary>Drawer extensions docked to the right side — rendered as toggle buttons in the appbar.</summary>
    private IEnumerable<IDrawerExtension> RightAnchorDrawerExtensions
        => Extensions.GetDrawerExtensions().Where(d => d.Anchor == DrawerAnchor.End);

    /// <summary>
    /// Aggregated navigation tree — every mounted <see cref="IWebsiteArea"/>'s
    /// <see cref="INavigationProvider.Get"/> result flattened into one
    /// <see cref="MudNavMenu"/>. Permission filtering happens inside
    /// <c>INavigationProvider</c> (it already knows the active user).
    /// </summary>
    private IReadOnlyList<NavGroup> NavGroups
    {
        get
        {
            if (CurrentSite.AreaKeys.Count == 0) return Array.Empty<NavGroup>();

            var groups = new List<NavGroup>();
            foreach (var areaKey in CurrentSite.AreaKeys)
                groups.AddRange(NavProvider.Get(areaKey));

            // Stable sort by Order so multi-area dashboards always present the
            // navigation in a deterministic order regardless of area registration
            // sequence.
            groups.Sort((a, b) => a.Order.CompareTo(b.Order));
            return groups;
        }
    }

    /// <summary>
    /// Maps the website-level <see cref="BreadcrumbsModel"/> sequence to MudBlazor's
    /// <see cref="BreadcrumbItem"/>. Returns an empty list (never null) so the
    /// markup can use <c>Count &gt; 0</c> uniformly.
    /// </summary>
    private List<BreadcrumbItem> BreadcrumbItems
        => BreadcrumbsProvider.Get() is { } items
            ? items.Select(c => new BreadcrumbItem(
                text: c.Text.Value,
                // Strip the leading slash so MudBreadcrumbs emits a RELATIVE href
                // that resolves against the active <base href>. With base
                // "/dashboard/" a crumb declared as Href="/components" then
                // navigates to "/dashboard/components" instead of falling through
                // to the root site's "/components" page (absolute paths bypass
                // <base href>). See Extensions/UrlPathRendering.cs.
                href: c.Href.ToHref(),
                disabled: c.Disabled,
                icon: c.Icon)).ToList()
            : [];

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        ThemeManager.OnChange += OnReactiveSourceChanged;
        DrawerStates.OnChange += OnReactiveSourceChanged;
        BreadcrumbsProvider.OnChange += OnReactiveSourceChanged;
        NavProvider.OnChanged += OnNavigationChanged;
        Tenant.OnChange += OnReactiveSourceChanged;
        Culture.OnChange += OnReactiveSourceChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        // Bridges Blazor Server's request-scope → circuit-scope cookie gap (the
        // request-scoped cookie repo doesn't carry over to the interactive
        // circuit). Safe — HydrateAsync is idempotent.
        await ThemeManager.HydrateAsync();

        // Subscribe to viewport so the drawer adapts to the actual window size
        // (MudBlazor breakpoints alone are not enough — we need the raw width to
        // pick between Responsive / Mini / closed).
        await BrowserViewport.SubscribeAsync(this, fireImmediately: true);
    }

    private void OnReactiveSourceChanged()
        => _ = InvokeAsync(StateHasChanged);

    // INavigationProvider.OnChanged carries an area-key payload; we don't filter
    // on it (the layout always re-renders the whole nav tree). The parameter is
    // named (not "_") to avoid colliding with the discard pattern below.
    private void OnNavigationChanged(string? areaKey)
        => _ = InvokeAsync(StateHasChanged);

    // ─── Event handlers ────────────────────────────────────────────────────────

    private void ToggleLeftDrawer() => _leftDrawerOpen = !_leftDrawerOpen;

    private void OnRightRailOpenChanged(bool open) => _rightRailOpen = open;

    private void ToggleDrawerExtension(string extensionId)
        => DrawerStates.GetState(extensionId)?.Toggle();

    // MudDrawer's OpenChanged event fires both ways (user clicked overlay → false;
    // we open programmatically → true). We forward to the canonical service so
    // the state survives even if MudDrawer's own internal flag gets out of sync.
    private void OnDrawerOpenChanged(string extensionId, bool open)
    {
        var state = DrawerStates.GetState(extensionId);
        if (state is null) return;
        if (open) state.Open();
        else state.Close();
    }

    public async ValueTask DisposeAsync()
    {
        ThemeManager.OnChange -= OnReactiveSourceChanged;
        DrawerStates.OnChange -= OnReactiveSourceChanged;
        BreadcrumbsProvider.OnChange -= OnReactiveSourceChanged;
        NavProvider.OnChanged -= OnNavigationChanged;
        Tenant.OnChange -= OnReactiveSourceChanged;
        Culture.OnChange -= OnReactiveSourceChanged;

        try
        {
            await BrowserViewport.UnsubscribeAsync(this);
        }
        catch (Exception)
        {
            // The viewport service may already be gone if the circuit died before
            // dispose ran; swallowing is OK — there is nothing we can do here and
            // throwing would mask the real circuit-termination error.
        }
    }

    // ---- Responsive drawer breakpoints ----
    async Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs args)
    {
        _viewportWidth = args.BrowserWindowSize.Width;

        // Same breakpoints as the legacy dashboard — they map cleanly to MudBlazor's
        // "sm / md / lg" thresholds without forcing us to inject IBreakpointService.
        switch (_viewportWidth)
        {
            case < 425:
                _leftDrawerOpen = false;
                _leftDrawerOpenMini = false;
                _leftDrawerVariant = DrawerVariant.Responsive;
                // Phone: rail goes Temporary (overlay) so it doesn't permanently
                // steal half the screen. User opens it via the appbar icons that
                // get re-shown by the rail-vs-icons guard in the .razor.
                _rightRailOpen = false;
                _rightRailVariant = DrawerVariant.Temporary;
                break;
            case < 1024:
                _leftDrawerOpenMini = true;
                _leftDrawerVariant = DrawerVariant.Mini;
                _rightRailOpen = false;
                _rightRailVariant = DrawerVariant.Temporary;
                break;
            default:
                _leftDrawerOpen = true;
                _leftDrawerOpenMini = false;
                _leftDrawerVariant = DrawerVariant.Responsive;
                _rightRailOpen = true;
                _rightRailVariant = DrawerVariant.Persistent;
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    // ---- Swipe gestures (mobile) ----
    private void OnSwipe(SwipeEventArgs args)
    {
        if (!Site.Layout.EnableSwipeGestures) return;
        // Only handle swipe on mobile-ish widths; on desktop swipe usually means
        // "select text" or "two-finger scroll" and stealing those gestures is rude.
        if (_viewportWidth >= 1024) return;

        switch (args.SwipeDirection)
        {
            case SwipeDirection.LeftToRight:
                _leftDrawerOpen = true;
                break;
            case SwipeDirection.RightToLeft:
                _leftDrawerOpen = false;
                break;
        }
    }

    // Static list of RTL culture prefixes — enough to cover the realistic locales
    // the dashboard ships with today (Arabic, Hebrew, Persian, Urdu). For broader
    // coverage replace with CultureInfo.TextInfo.IsRightToLeft, but that pulls in
    // a full CultureInfo allocation per render — the prefix check is allocation-free.
    private static bool IsRtlCulture(string culture)
    {
        if (string.IsNullOrEmpty(culture)) return false;
        return culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase)
            || culture.StartsWith("he", StringComparison.OrdinalIgnoreCase)
            || culture.StartsWith("fa", StringComparison.OrdinalIgnoreCase)
            || culture.StartsWith("ur", StringComparison.OrdinalIgnoreCase);
    }
}
