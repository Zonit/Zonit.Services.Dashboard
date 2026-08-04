using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using MudBlazor.Services;
using Zonit.Dashboard.Extensions;
using Zonit.Dashboard.Themes;
using Zonit.Extensions.Auth;
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
    [Inject] private IAuthenticatedProvider Authenticated { get; set; } = default!;

    // Distinct from NavProvider above (that one supplies the tree; this one reports where the
    // browser currently is). The sidebar needs the second: RenderNavGroup decides whether a
    // branch renders expanded from the active path, and Blazor's NavLink only re-renders
    // itself on navigation — the surrounding <details open> would keep its stale value.
    [Inject] private NavigationManager BrowserNavigation { get; set; } = default!;

    // Single piece of layout-local state — every other "open" flag belongs to a
    // service (drawer states in IExtensionDrawerStates, theme mode in IThemeManager).
    // The left drawer has no dedicated service because there's exactly one — it's
    // simpler to keep its open/close in the layout than to introduce a service for
    // a single boolean.
    private bool _leftDrawerOpen = true;

    // Persistent right rail (user/theme/org/project/culture inline switchers).
    // Default open on desktop; the viewport observer flips it to closed on
    // narrow windows where there's no room for two persistent drawers side by side.
    private bool _rightRailOpen = true;
    private DrawerVariant _rightRailVariant = DrawerVariant.Persistent;

    // Responsive drawer state, driven by IBrowserViewportObserver below.
    private DrawerVariant _leftDrawerVariant = DrawerVariant.Persistent;
    private int _viewportWidth = 1920;

    // Below this the left drawer becomes a temporary overlay instead of stealing a
    // permanent column. 960 is MudBlazor's "md" edge and also roughly where a 240px
    // drawer stops leaving a usable content area.
    private const int NavBreakpoint = 960;

    // The rail needs its own, higher, threshold: nav + content + rail is three columns,
    // and below ~1280 the middle one gets squeezed to the point of being unreadable.
    private const int RailBreakpoint = 1280;

    // Whether the rail is currently a permanent column rather than an overlay. Drives
    // the single appbar button — no point offering to open something already on screen.
    private bool _railIsPersistent = true;

    // Which responsive band the last viewport report landed in. The observer fires on
    // every resize frame, not only on breakpoint crossings, so the forced open/closed
    // states below are applied ONLY when the band actually changes. Re-applying them on
    // each frame is what made a drawer the visitor had just closed spring back open the
    // moment they nudged the window edge.
    private int _band = -1;

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
    /// <see cref="INavigationProvider.Get"/> result, filtered to what the current identity may
    /// see, in one list.
    /// </summary>
    /// <remarks>
    /// <para><b>Permission filtering happens here, not in the provider.</b> This comment used to
    /// claim the opposite — "filtering happens inside INavigationProvider (it already knows the
    /// active user)" — and it was wrong on both counts: <c>NavigationService.Get</c> filters by
    /// Site only and documents that permissions are deliberately left to the UI layer, and it
    /// never sees an identity at all. Measured before the fix: an anonymous visitor with no
    /// permissions saw every gated group in the menu.</para>
    ///
    /// <para>Deep nodes are filtered during rendering rather than here, because hiding a leaf
    /// three levels down would otherwise mean rebuilding every ancestor — see
    /// <see cref="NavPermissions"/>. This pass only drops whole top-level groups.</para>
    /// </remarks>
    private IReadOnlyList<NavGroup> NavGroups
    {
        get
        {
            if (CurrentSite.AreaKeys.Count == 0) return Array.Empty<NavGroup>();

            var identity = Authenticated.Current;
            var groups = new List<NavGroup>();
            foreach (var areaKey in CurrentSite.AreaKeys)
                foreach (var group in NavProvider.Get(areaKey))
                    if (NavPermissions.IsVisible(group, identity))
                        groups.Add(group);

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
        // The menu is now a function of the identity, so a sign-in or sign-out has to
        // repaint it — otherwise a visitor who just signed in keeps the anonymous menu
        // until they reload, which reads as the permissions not having taken effect.
        Authenticated.OnChange += OnReactiveSourceChanged;
        BrowserNavigation.LocationChanged += OnLocationChanged;
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

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // A tap on a nav link should not leave the overlay covering the page it just
        // opened. Docked drawers stay put — there is nothing to get out of the way of.
        if (_leftDrawerVariant == DrawerVariant.Temporary) _leftDrawerOpen = false;
        if (_rightRailVariant == DrawerVariant.Temporary) _rightRailOpen = false;

        _ = InvokeAsync(StateHasChanged);
    }

    // ─── Event handlers ────────────────────────────────────────────────────────

    private void ToggleLeftDrawer()
    {
        _leftDrawerOpen = !_leftDrawerOpen;

        // Two overlays stacked on a phone is a dead end — the one underneath is
        // unreachable and its scrim swallows the taps meant for it.
        if (_leftDrawerOpen && _leftDrawerVariant == DrawerVariant.Temporary)
            _rightRailOpen = false;
    }

    private void ToggleRightRail()
    {
        _rightRailOpen = !_rightRailOpen;

        if (_rightRailOpen && _rightRailVariant == DrawerVariant.Temporary)
            _leftDrawerOpen = false;
    }

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
        Authenticated.OnChange -= OnReactiveSourceChanged;
        BrowserNavigation.LocationChanged -= OnLocationChanged;

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

        // Three bands: phone-ish (nav overlays), laptop (nav docked, rail overlays),
        // wide (both docked). No mini band — see the comment on the left MudDrawer.
        var band = _viewportWidth switch
        {
            < NavBreakpoint => 0,
            < RailBreakpoint => 1,
            _ => 2,
        };

        _railIsPersistent = band == 2;

        if (band == _band)
            return;

        _band = band;

        switch (band)
        {
            case 0:
                _leftDrawerVariant = DrawerVariant.Temporary;
                _leftDrawerOpen = false;
                _rightRailVariant = DrawerVariant.Temporary;
                _rightRailOpen = false;
                break;
            case 1:
                _leftDrawerVariant = DrawerVariant.Persistent;
                _leftDrawerOpen = true;
                _rightRailVariant = DrawerVariant.Temporary;
                _rightRailOpen = false;
                break;
            default:
                _leftDrawerVariant = DrawerVariant.Persistent;
                _leftDrawerOpen = true;
                _rightRailVariant = DrawerVariant.Persistent;
                _rightRailOpen = true;
                break;
        }

        await InvokeAsync(StateHasChanged);
    }

    // ---- Swipe gestures (mobile) ----
    private void OnSwipe(SwipeEventArgs args)
    {
        if (!Site.Layout.EnableSwipeGestures) return;
        // Only handle swipe where a drawer is actually an overlay; on a docked layout
        // swipe usually means "select text" or "two-finger scroll" and stealing those
        // gestures is rude.
        if (_viewportWidth >= NavBreakpoint) return;

        switch (args.SwipeDirection)
        {
            case SwipeDirection.LeftToRight:
                // Right-hand overlay wins the gesture when it is the one on screen,
                // otherwise a swipe meant to dismiss the rail would open the nav behind it.
                if (_rightRailOpen) _rightRailOpen = false;
                else _leftDrawerOpen = true;
                break;
            case SwipeDirection.RightToLeft:
                if (_leftDrawerOpen) _leftDrawerOpen = false;
                else if (Site.Layout.ShowRightRail) _rightRailOpen = true;
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
