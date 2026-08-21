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
    [Inject] private IPageMetaState PageMeta { get; set; } = default!;
    [Inject] private ILayoutContext LayoutContext { get; set; } = default!;
    [Inject] private ICultureProvider Culture { get; set; } = default!;
    // Distinct from Culture above: that one translates, this one enumerates. The supported-language
    // list is where a language's own metadata lives (direction, display name, flag).
    [Inject] private ICultureState CultureState { get; set; } = default!;
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
    //
    // Answered by the culture package's own LanguageModel.IsRightToLeft rather than by a prefix
    // list kept here. The list this replaced covered ar/he/fa/ur and nothing else, so a host that
    // registered any other RTL language — Kurdish, Pashto, Divehi, Yiddish — got a left-to-right
    // dashboard with right-to-left text in it, and the only place stating that language's
    // direction had already been asked and ignored.
    private bool IsRtl
    {
        get
        {
            if (!Culture.Current.HasValue) return false;
            var code = Culture.Current.ValueOrDefault;

            foreach (var lang in CultureState.Supported)
                if (string.Equals(lang.Code, code, StringComparison.OrdinalIgnoreCase))
                    return lang.IsRightToLeft;

            return IsRtlCulture(code);
        }
    }

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
    /// <summary>
    /// The trail the page pushed, or empty. Returned as the framework's own model rather than
    /// MudBlazor's: the markup decides what is a link and what is text, and a projection through
    /// <c>BreadcrumbItem</c> would have to encode that decision in a flag first.
    /// </summary>
    private IReadOnlyList<BreadcrumbsModel> BreadcrumbTrail
        => BreadcrumbsProvider.Get() ?? [];

    /// <summary>
    /// <see langword="true"/> when the last crumb says the same thing as the <c>&lt;h1&gt;</c>
    /// directly beneath it, so the trail should stop one short.
    /// </summary>
    /// <remarks>
    /// <para>A page pushes "Cultures / Cultures — overview" and states its title as "Cultures —
    /// overview", and the chrome printed both, one under the other, two lines apart. Every page in
    /// the demo host does this, because it is what the two APIs each ask for on their own: the
    /// trail wants to end at the current page, and the heading names the current page.</para>
    ///
    /// <para>Resolved in favour of the heading — it is the larger, more legible copy, and a trail
    /// that ends at the last <em>link</em> is still a complete answer to "where am I". The crumb is
    /// dropped only when the strings match after translation, so a trail whose leaf genuinely says
    /// something else keeps it.</para>
    /// </remarks>
    private bool TitleReplacesLastCrumb
    {
        get
        {
            if (!Site.Layout.ShowPageTitle) return false;
            if (PageTitle is not { Length: > 0 } title) return false;
            if (BreadcrumbTrail.Count == 0) return false;

            var last = BreadcrumbTrail[^1];
            return string.Equals(
                Culture.Translate(last.Text.Value).Value,
                title,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>The trail as rendered — see <see cref="TitleReplacesLastCrumb"/>.</summary>
    private IReadOnlyList<BreadcrumbsModel> VisibleCrumbs
    {
        get
        {
            var trail = BreadcrumbTrail;
            if (!TitleReplacesLastCrumb) return trail;

            // A one-crumb trail that duplicates the heading has nothing left to show, and an empty
            // <nav> is worse than no <nav>.
            return trail.Count <= 1 ? [] : trail.Take(trail.Count - 1).ToArray();
        }
    }

    /// <summary>
    /// The page's own title, for the <c>&lt;h1&gt;</c> above its body.
    /// </summary>
    /// <remarks>
    /// <para>The <em>page's</em> title, not the composed document title — the browser tab wants
    /// "Users - Acme", a heading does not. <see cref="PageMeta"/> holds the uncomposed value, which
    /// is why this reads it rather than anything the head produced.</para>
    ///
    /// <para><b>Translated here, because this is the endpoint.</b> A page states its title as the
    /// raw English string — that string <em>is</em> the translation key in this framework — and
    /// every place that renders it looks up a rendition. The SEO layer already did this for the
    /// document title and Open Graph tags; a heading that skipped it would print English inside an
    /// otherwise Polish page, and only on the dashboard. Text with no rendition falls through to
    /// itself, so this is safe for a value that was never meant to be translated.</para>
    /// </remarks>
    private string? PageTitle => PageMeta.Current?.Title is { Length: > 0 } title
        ? Culture.Translate(title).Value
        : null;

    /// <summary>
    /// The content container's width, from whatever the routed page asked for.
    /// </summary>
    /// <remarks>
    /// <para><see cref="PageWidth.Content"/> — the value a page gets when it says nothing — maps to
    /// <c>MaxWidth.False</c>, which is precisely what this container was hard-coded to before. That
    /// is the point: turning the feature on moves nothing that existed.</para>
    ///
    /// <para><see cref="PageWidth.Wide"/> lands on the same value, and that is not an omission. A
    /// dashboard page is full-bleed by nature, so the distinction the enum draws for a content site
    /// has nothing to separate here; the mapping earns its keep at the narrow end, where a settings
    /// form stops being one input stretched across an ultrawide monitor.</para>
    /// </remarks>
    private MaxWidth ContentWidth => Site.Layout.HonourPageWidth
        ? LayoutContext.Width switch
        {
            PageWidth.Narrow => MaxWidth.Small,
            PageWidth.Reading => MaxWidth.Medium,
            PageWidth.Wide => MaxWidth.False,
            PageWidth.Full => MaxWidth.False,
            _ => MaxWidth.False,
        }
        : MaxWidth.False;

    // ─── Lifecycle ─────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        // Both of these change while the page is already rendered — a title assigned after an
        // await, a width decided once the record is loaded — so the layout has to repaint on them
        // exactly as it does on breadcrumbs.
        PageMeta.OnChange += OnReactiveSourceChanged;
        LayoutContext.OnChange += OnReactiveSourceChanged;

        ThemeManager.OnChange += OnReactiveSourceChanged;
        DrawerStates.OnChange += OnDrawerStatesChanged;
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

    private void OnDrawerStatesChanged()
    {
        SyncOverlayDrawers();
        _ = InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Enforces "at most one overlay open on the right" whenever a drawer extension's state
    /// changes.
    /// </summary>
    /// <remarks>
    /// <para>The rail and the extension panels are all <c>Anchor=End</c> <c>MudDrawer</c>s and
    /// MudBlazor stamps them all with the same z-index, so which one the visitor actually sees is
    /// decided by document order — and the rail is last. On a phone, tapping "Language" in the rail
    /// opened the 320px language panel <em>underneath</em> the 220px rail: measured, only the
    /// leftmost 100px of the panel was visible or clickable, and both panels drew their own scrim
    /// so the page behind went double-dark. The reported symptom — "the right-hand navigation
    /// overlaps the slide-out and it stops being clickable" — is this.</para>
    ///
    /// <para>Enforced here rather than at each call site because a drawer can be opened from four
    /// places (a rail row, an appbar button, a toolbar extension, a panel's own link) and only this
    /// one is guaranteed to run for all of them. Docked rails are left alone — a persistent rail is
    /// a column beside the panel, not on top of it, and closing it on a wide screen would make the
    /// chrome jump for no reason.</para>
    /// </remarks>
    private void SyncOverlayDrawers()
    {
        if (_rightRailVariant != DrawerVariant.Temporary || !_rightRailOpen)
            return;

        foreach (var ext in RightAnchorDrawerExtensions)
        {
            if (DrawerStates.GetState(ext.Id)?.IsOpen == true)
            {
                _rightRailOpen = false;
                return;
            }
        }
    }

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

        // Extension panels are always overlays, at every width, so they always have to go. The
        // built-in panels close themselves when their own links navigate; nothing was closing them
        // when the visitor picked a page out of the sidebar behind them instead.
        DrawerStates.CloseAll(DrawerAnchor.End);

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
        {
            _leftDrawerOpen = false;

            // The other half of SyncOverlayDrawers: a panel left open behind the rail would be
            // reachable only by its own scrim, and the visitor would be tapping a language list
            // they cannot see.
            DrawerStates.CloseAll(DrawerAnchor.End);
        }
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
        PageMeta.OnChange -= OnReactiveSourceChanged;
        LayoutContext.OnChange -= OnReactiveSourceChanged;
        ThemeManager.OnChange -= OnReactiveSourceChanged;
        DrawerStates.OnChange -= OnDrawerStatesChanged;
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

        // Only handle swipe where a drawer is actually an overlay; on a docked layout swipe
        // usually means "select text" or "two-finger scroll" and stealing those gestures is rude.
        //
        // Tested per drawer rather than against NavBreakpoint. The rail is still an overlay
        // between 960 and 1280, and an extension panel is an overlay at EVERY width, so the old
        // single width test disabled the gesture for both of them across the entire laptop band
        // while claiming to be about "a docked layout".
        var navIsOverlay = _leftDrawerVariant == DrawerVariant.Temporary;
        var railIsOverlay = _rightRailVariant == DrawerVariant.Temporary;
        var panelOpen = RightAnchorDrawerExtensions.Any(e => DrawerStates.GetState(e.Id)?.IsOpen == true);

        switch (args.SwipeDirection)
        {
            case SwipeDirection.LeftToRight:
                // Dismiss what is on the right, innermost first. The panel was invisible to this
                // handler before, so a panel could never be swiped away — and the rail could be
                // dismissed out from under one, leaving the panel stranded with no visible chrome.
                if (panelOpen) DrawerStates.CloseAll(DrawerAnchor.End);
                else if (railIsOverlay && _rightRailOpen) _rightRailOpen = false;
                else if (navIsOverlay) _leftDrawerOpen = true;
                break;

            case SwipeDirection.RightToLeft:
                if (navIsOverlay && _leftDrawerOpen) _leftDrawerOpen = false;
                // Never summon the rail on top of an open panel — that is the overlap bug,
                // reproduced by gesture.
                else if (railIsOverlay && Site.Layout.ShowRightRail && !panelOpen) _rightRailOpen = true;
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
