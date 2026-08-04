# Zonit.Dashboard — mounting and extending an admin UI

Zonit.Dashboard is an **overlay on `Zonit.Extensions.Website`**, not a second framework:

```csharp
// what UseDashboard actually is
app.UseWebsite<DashboardApp, DashboardSiteOptions>(directory, configure);
```

Everything the web kernel provides still applies — areas, layouts, `PageBase`, navigation,
breadcrumbs, toasts, `[RequirePermission]`, several mounts per app. Dashboard supplies the chrome
(MudBlazor appbar, drawers, theming) and slots for contributing to it.

## Read this first: three traps

**1. Never hand-write `App.razor` or `Routes.razor` for a dashboard mount.** Both ship inside the
package. `DashboardApp.razor` emits the document shell: a `<base href>` correct for the mount,
MudBlazor CSS and fonts, `<title>` / theme-colour / colour-scheme from the tenant, the zero-flicker
theme script, `<WebsiteHydrator/>` and `<Routes/>`. Writing your own is not "customising" — it is
re-implementing invariants and you will get the base href wrong under a non-root mount, which breaks
client-side routing on the first navigation.

**2. `AddDashboard()` takes no arguments.**

```csharp
builder.Services.AddDashboard();                      // correct
builder.Services.AddDashboard(o => o.Directory = …);  // does not compile
```

Per-mount configuration belongs to `UseDashboard`, because one registration can back several mounts.

**3. Non-root mounts must be declared before the root mount.** The root mount ends in terminal
endpoint middleware, so any branch registered after it is unreachable. The framework throws a
developer-actionable exception rather than serving 404s.

## Minimal host

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebsite(o => o.AddArea<AdminArea>());
builder.Services.AddDashboard();

var app = builder.Build();

app.UseDashboard("/admin", o =>
{
    o.Permission = "admin";
    o.AddArea<AdminArea>();
});

app.Run();
```

`AddDashboard()` calls `AddWebsite()` internally and is idempotent, so ordering against your own
`AddWebsite(...)` does not matter. What it registers: MudBlazor services and translations,
`IDashboardCurrentSite` (scoped, populated by the per-mount middleware), the four layout keys below,
a `IToastProvider` adapter onto MudBlazor's snackbar, and the built-in toolbar/drawer extensions.

## Dashboard alongside a public site

The common shape — a marketing root and an admin area, in one app, sharing areas and auth:

```csharp
builder.Services.AddWebsite(o =>
{
    o.AddArea<PublicArea>();
    o.AddArea<AdminArea>();
});
builder.Services.AddDashboard();

var app = builder.Build();

// non-root FIRST
app.UseDashboard("/admin", o =>
{
    o.Permission = "admin";
    o.AddArea<AdminArea>();
    o.Theme.PrimaryColor = "#c62828";     // red chrome so nobody mistakes prod admin for the site
    o.Layout.LeftDrawerWidth = 260;
});

// root LAST, plain Website - no dashboard chrome
app.UseWebsite<App>("/", o =>
{
    o.Mode = WebsiteMode.Server;
    o.AddArea<PublicArea>();
});

app.Run();
```

Only the root mount needs your own `App.razor`. The `/admin` mount uses the bundled one.

## Pages

Ordinary Website pages in an area you mounted — nothing dashboard-specific, and they never
reference this assembly:

```razor
@page "/users"
@inherits PageBase
@attribute [RequirePermission("users.read")]

<PageHeader Title="@T("Users")" />

<MudTable Items="_users" Dense="true">
    <HeaderContent><MudTh>@T("Name")</MudTh></HeaderContent>
    <RowTemplate><MudTd>@context.Name.Value</MudTd></RowTemplate>
</MudTable>
```

Note `@context.Name.Value` — `Name` is a `Title` value object, so string operations need `.Value`.

## Layout keys

| Key | Chrome |
| --- | --- |
| `Dashboard.Main` | full — appbar, nav drawer, extension drawers, breadcrumbs (default) |
| `Dashboard.Minimal` | appbar + content, no nav drawer — login, onboarding |
| `Dashboard.Empty` | providers + `@Body` only — full-screen views |
| `Zonit.Minimal` | overwritten so framework error pages match the dashboard |

```razor
@attribute [LayoutKey("Dashboard.Empty")]
```

The key is a string, resolved through the Website layout registry, so a plug-in area can opt into
dashboard chrome without a package reference to Zonit.Dashboard.

## Navigation

An area returns `IReadOnlyList<NavGroup>`. Groups nest through `Groups`, links nest through
`NavItem.Children`, and both are rendered to arbitrary depth:

```csharp
public IReadOnlyList<NavGroup> Navigation { get; } =
[
    new NavGroup
    {
        Title = "Auth",
        Order = 20,
        Children =
        [
            new NavItem
            {
                Title = "Identity",
                Url = "/auth",
                Children =
                [
                    new NavItem { Title = "AuthorizeView", Url = "/auth/authorize-view" },
                    new NavItem
                    {
                        Title = "Permissions",
                        Url = "/auth/permissions",
                        Children =
                        [
                            new NavItem { Title = "Sandbox", Url = "/auth/sandbox" },
                        ],
                    },
                ],
            },
        ],
    },
];
```

What the renderer does with that:

- **A branch containing the current page renders expanded**, all the way up. You never land on a
  page whose ancestry is collapsed, so the sidebar always shows where you are. `Expanded = true`
  forces a group open on top of that.
- **A `NavItem` with children is both a link and a disclosure.** Activating it navigates *and*
  expands; there is no separate chevron button, because a second hit target would need JavaScript
  and this tree is built to work during prerender with no circuit.
- **Indentation tapers with depth** rather than adding a fixed step per level, and stops
  increasing past level four. Fixed steps overflow a 240px drawer at about the third level and
  turn every label into an ellipsis.
- **Prefix matching is segment-aware.** `Match = false` lights up `/vo` for `/vo/strings` but not
  for `/voice`.

Hrefs go through `UrlPath.ToHref()`, which strips the leading slash so the browser resolves them
against `<base href>`. Declare `Url = "/auth"` and a dashboard mounted at `/admin` links to
`/admin/auth`, not to the root site's `/auth`.

### Icons

`NavGroup.Icon` and `NavItem.Icon` take **SVG markup**, not an icon name.

```csharp
Icon = Icons.Material.Filled.Newspaper   // ✔ MudBlazor constant — inner SVG fragment
Icon = "<svg viewBox=\"0 0 24 24\">…</svg>"  // ✔ complete document, hand-written
Icon = "Newspaper"                        // ✘ a NAME — dropped, no icon rendered
```

Both markup shapes work: a complete `<svg>` passes through, and a bare fragment (`<g>`, `<path>`,
`<rect>` — which is what every `Icons.Material.Filled.*` constant actually is) gets wrapped in a
`24x24` viewBox. A value that is not markup cannot be resolved to a glyph without reflecting over
the ~2000 constants in `Icons.Material.Filled`, which would forfeit the trimming guarantees this
package makes, so it is dropped instead.

Passing a name used to be worse than useless: the string landed in the DOM as literal text inside
an 18px box and painted over the label beside it, so `Icon = "Newspaper"` on one group visibly
corrupted its neighbours. That value is now discarded before it reaches the DOM, and the icon box
clips as a second line of defence — but the icon is still missing, so pass the constant.

Icons are optional, and partly-iconned lists still line up: when at least one entry in a list has
an icon, the others get an empty slot of the same width, so every label in that list shares one
left edge. A list where nobody declares an icon keeps its labels flush left instead of carrying a
column of nothing.

### Permissions

`NavGroup.Permission` and `NavItem.Permission` are honoured by the sidebar: a node the current
identity cannot satisfy is not rendered, and a group left with no visible children disappears with
them. A group that carries its own `Link` survives — it is a destination, not just a container.

Matching goes through `Identity.HasPermission`, so wildcards behave exactly as they do everywhere
else in the grammar. Measured against a node requiring `settings.write`:

| Granted | Node shown |
| --- | --- |
| *(anonymous)* | no |
| `users.read` | no |
| `*` | **no** — one `*` segment matches one segment, so this implies `settings`, not `settings.write` |
| `settings.*` | yes |
| `*.*` | yes |

Two things to be clear about:

- **This is cosmetic, not access control.** The kernel's `INavigationProvider` filters by Site
  only and deliberately leaves permissions to the UI layer. Guard the destination with
  `SiteOptions.Permission` or `[RequirePermission]`; hiding the link stops a user being offered a
  door they cannot open, nothing more.
- **A host rendering its own navigation gets none of this.** It is the dashboard chrome that
  filters, not the framework.

### Responsive behaviour

Three bands, driven by window width:

| Width | Navigation | Right rail |
| --- | --- | --- |
| `< 960` | temporary overlay, closed; hamburger or swipe opens it | temporary overlay, one appbar button opens it |
| `960 – 1279` | docked, open | temporary overlay |
| `≥ 1280` | docked, open | docked, open |

Notes worth knowing before you change these:

- There is no icon-rail (mini) band. It requires every nav item to carry an icon, and MudBlazor's
  mini stylesheet only knows how to fold `MudNavMenu`'s own markup into 56px — this tree is plain
  HTML, so a mini band renders full-width text rows clipped to 40px.
- Only one overlay is ever open at a time; opening either closes the other, and navigating closes
  whichever is covering the page.
- Forced open/closed states are applied when the band *changes*, not on every resize frame, so a
  drawer you closed by hand stays closed while you drag the window edge.

## The mount owns the document

`UseDashboard` renders the package's own `DashboardApp`, which emits the entire HTML document.
**Your application's `App.razor` is not involved**, so nothing you put in its `<head>` reaches a
dashboard mount: web fonts, an icon-font stylesheet, a Tailwind CDN tag, `app.css`, analytics.
Repeat whatever the mount needs through `CustomSnippet`:

```csharp
app.UseDashboard("/management", o =>
{
    o.AddArea<ManagementArea>();
    o.CustomSnippet = """
        <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
        <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&display=swap">
        """;
});
```

One thing is **not** left to memory: the Blazor scoped-CSS bundle, `{ApplicationName}.styles.css`.
`DashboardApp` links it automatically, because forgetting it disables CSS isolation across the
whole application at once — every `*.razor.css` in the host and in every RCL it references stops
applying, while components still render, scope attributes are still emitted and nothing 404s or
logs. The symptom is "the styling regressed" with no thread to pull. The link is skipped when the
build produced no bundle, so a host with no scoped CSS does not pay a 404 for it.

## Per-mount options

`DashboardSiteOptions` derives from `SiteOptions`, so the whole base surface is available
(`Permission`, `Mode`, `Compression`, `HttpsRedirection`, `ExceptionHandlerPath`, `AddArea<T>()`, …)
plus:

| Member | Default | Purpose |
| --- | --- | --- |
| `Layout.ShowLeftDrawer` | `true` | navigation drawer |
| `Layout.ShowRightDrawer` | `true` | extension drawers |
| `Layout.ShowRightRail` | `true` | icon rail |
| `Layout.LeftDrawerWidth` | `240` | px |
| `Layout.RightDrawerWidth` | `280` | px |
| `Layout.RightRailWidth` | `220` | px |
| `Layout.ShowBreadcrumbs` | `true` | |
| `Layout.ShowProgressBar` | `true` | appbar progress for long-running tasks |
| `Layout.EnableSwipeGestures` | `true` | mobile drawer swipe |
| `Layout.AppbarElevation` | `1` | MudBlazor elevation |
| `Theme` | — | per-mount colour overrides layered over `Tenant.Settings.Theme` |
| `ExtensionsWhitelist` | `null` (all) | restrict which extensions appear on this mount |
| `CustomSnippet` | `null` | markup injected into `<head>` |

## Toolbar and drawer extensions

Contribute chrome from any assembly:

```csharp
public sealed class AlertsToolbar : ToolbarExtensionBase<AlertsButton>
{
    public override string Id   => "alerts";
    public override string Name => "Alerts";
    public override ToolbarPosition Position => ToolbarPosition.End;
}

builder.Services.AddToolbarExtension<AlertsToolbar>();
```

`DrawerExtensionBase<TComponent>` is the same shape for the right-hand drawer. Both render
`TComponent` into their slot; override `ConfigureComponent(builder)` to pass parameters.

Registered by `AddDashboard()` already: user profile, theme selector, culture switcher, workspace
switcher, project switcher. Task-manager toolbar and drawer are opt-in:

```csharp
builder.Services.AddDashboardTaskManager();   // needs Zonit.Messaging.Tasks
```

Use `ExtensionsWhitelist` to show a subset on a given mount.

## Theming

Colours resolve in layers: `DashboardSiteOptions.Theme` (per mount) over `Tenant.Settings.Theme`
(per tenant brand). Light / Dark / Auto is `IThemeManager`, persisted by cookie and read during SSR,
so there is no flash of the wrong theme on first paint. `AddDashboardTheme<T>()` registers a custom
`IDashboardTheme`.

## Trimming and AOT

`IsTrimmable` yes, Native AOT **no**. Dashboard's own code emits zero IL warnings, but Blazor's
`Router` / `LayoutView` (IL2111 / IL2110, raised inside the Razor-generated `Routes_razor.g.cs`) and
MudBlazor (IL3050 / IL2075) are not AOT-clean. `AddDashboard()` therefore carries
`[RequiresUnreferencedCode]` / `[RequiresDynamicCode]`, so publishing AOT gives you a build-time
diagnostic rather than a runtime failure.

## See also

- `.zonit/extensions/website/hosting.md` — `AddWebsite` / `UseWebsite`, `SiteOptions`, mount ordering
- `.zonit/extensions/website/areas.md` — writing the areas you mount here
- `.zonit/extensions/website/layouts.md` — how layout keys resolve
- `.zonit/extensions/tenants/tenants.md` — where `Tenant.Settings.Theme` comes from
