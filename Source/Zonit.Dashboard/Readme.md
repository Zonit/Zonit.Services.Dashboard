# Zonit.Dashboard

An admin dashboard **overlay on `Zonit.Extensions.Website`** — not a parallel framework.
`UseDashboard(...)` is literally:

```csharp
app.UseWebsite<DashboardApp, DashboardSiteOptions>(directory, configure);
```

So everything the web kernel does still applies: areas, layouts, `PageBase`, navigation,
breadcrumbs, toasts, `[RequirePermission]`, several mounts in one app. Dashboard adds the chrome —
MudBlazor appbar, drawers, theming — and slots for contributing to it.

[![NuGet](https://img.shields.io/nuget/v/Zonit.Services.Dashboard.svg)](https://www.nuget.org/packages/Zonit.Services.Dashboard/)

```bash
dotnet add package Zonit.Services.Dashboard
```

## You do not write App.razor or Routes.razor

Both ship inside the package. `DashboardApp.razor` emits the document shell — a `<base href>` that
is correct for the mount, MudBlazor CSS and fonts, theme colour and colour-scheme meta taken from
the tenant, the zero-flicker theme script, `<WebsiteHydrator/>` and `<Routes/>`. `Routes.razor`
wires the router against the assemblies registered for that mount.

That is deliberate: a dashboard shell is all but identical between applications, and the parts that
differ are configuration, not markup. You supply pages; the package owns the shell.

## Minimal host

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebsite(o => o.AddArea<AdminArea>());
builder.Services.AddDashboard();          // MudBlazor + chrome + the layout keys below

var app = builder.Build();

app.UseDashboard("/admin", o =>
{
    o.Permission = "admin";               // the whole mount behind one permission
    o.AddArea<AdminArea>();
    o.Layout.LeftDrawerWidth = 260;
});

app.Run();
```

`AddDashboard()` takes no arguments — per-mount configuration belongs to `UseDashboard`, because the
same registration can back several mounts. It calls `AddWebsite()` internally and is idempotent, so
ordering against your own `AddWebsite(...)` does not matter.

## A page

Nothing dashboard-specific — an ordinary Website page in an area you mounted:

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

## Layouts

`AddDashboard()` registers four keys in the Website layout registry:

| Key | Chrome |
| --- | --- |
| `Dashboard.Main` | full — appbar, nav drawer, extension drawers, breadcrumbs (the default) |
| `Dashboard.Minimal` | appbar + content, no nav drawer — login, onboarding, errors |
| `Dashboard.Empty` | providers + `@Body` only — full-screen views |
| `Zonit.Minimal` | overwritten so framework error pages match the dashboard look |

Choose one per page with `@attribute [LayoutKey("Dashboard.Empty")]`. Pages never reference this
assembly — the key is a string.

## Per-mount configuration

`DashboardSiteOptions` derives from `SiteOptions`, so the whole base surface (`Permission`, `Mode`,
`Compression`, `HttpsRedirection`, `ExceptionHandlerPath`, `AddArea<T>()`, …) is available, plus:

| Member | Purpose |
| --- | --- |
| `Layout` | `ShowLeftDrawer` / `ShowRightDrawer` / `ShowRightRail` and their widths, `ShowBreadcrumbs`, `ShowProgressBar`, `EnableSwipeGestures`, `AppbarElevation` |
| `Theme` | per-mount colour overrides layered over `Tenant.Settings.Theme` |
| `ExtensionsWhitelist` | restrict which toolbar/drawer extensions appear on this mount |
| `CustomSnippet` | markup injected into `<head>` |

Mounts are independent: an `/admin` with a red appbar and a `/support` with a grey one, carrying
different areas, both inheriting the tenant brand as their baseline.

> Declare every non-root mount **before** the root mount. The root mount ends in terminal endpoint
> middleware, so a branch registered after it is unreachable. The framework fails fast if you get
> this wrong.

## Extending the chrome

Contribute a toolbar button or a drawer panel from any assembly:

```csharp
public sealed class AlertsToolbar : ToolbarExtensionBase<AlertsButton>
{
    public override string Id   => "alerts";
    public override string Name => "Alerts";
    public override ToolbarPosition Position => ToolbarPosition.End;
}

builder.Services.AddToolbarExtension<AlertsToolbar>();
```

Registered by `AddDashboard()` out of the box: user profile, theme selector, culture switcher,
workspace switcher, project switcher. The task-manager toolbar and drawer are opt-in through
`AddDashboardTaskManager()` (requires `Zonit.Messaging.Tasks`).

Theming: `IThemeManager` with Light / Dark / Auto persisted by cookie and read during SSR, so there
is no flash of the wrong theme. Colours come from `Tenant.Settings.Theme`, overridden per mount by
`DashboardSiteOptions.Theme`.

## Trimming and AOT

`IsTrimmable` yes; Native AOT **no**, and the package declares that rather than shipping a badge it
cannot honour. Dashboard's own code produces zero IL warnings, but Blazor's `Router` / `LayoutView`
and MudBlazor are not AOT-clean, so `AddDashboard()` carries
`[RequiresUnreferencedCode]` / `[RequiresDynamicCode]` — you get a build-time diagnostic instead of
a runtime failure.

## Migrating from 0.1.x

The four-project layout (`Zonit.Services.Dashboard{,.Abstractions,.Application,.Components}`) is
gone, replaced by this single package.

| Removed | Use instead |
| --- | --- |
| `IExtensionManager`, `IDrawer`, `IArea` | `IDrawerExtension` / `IToolbarExtension` here, `IWebsiteArea` from Website |
| `INavigationManager`, `NavigationItem` | `IWebsiteArea.Navigation` with `NavGroup` / `NavItem` |
| `ISettingsManager`, `ISettingsExtension` | `ITenantProvider` and `Setting<T>` from `Zonit.Extensions.Tenants` |
| `DashboardSettings.Title` / `FavIcon` / `ThemeColor` / `Theme.*` | `Tenant.Settings.Site` and `Tenant.Settings.Theme` |
| `UseDashboardServices<T>` | `app.UseDashboard(directory, configure)` |

## License

MIT.
