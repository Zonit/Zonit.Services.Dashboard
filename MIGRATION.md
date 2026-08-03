# Zonit.Dashboard — Audyt i plan migracji z Zonit.Services.Dashboard (legacy)

Audyt wykonany na podstawie pełnego przeglądu obu drzew (`Source/Zonit.Services.Dashboard*` vs `Source/Zonit.Dashboard`).
Cel: przenieść 100% funkcji UI ze starego Dashboardu do nowego (opartego na `Zonit.Extensions.Website`),
**bez pominięć**. Ulepszenia robimy w kroku następnym.

> **Konwencja statusów**: ✅ done · 🚧 in progress · ❌ todo · ❎ świadomie pominięte

## 📊 Status / progress checkpoint

**Sesja 2026‑05‑17 (rozszerzona)** — build `Zonit.Dashboard.csproj` = **0 errors / 0 nowych warningów**.

| Punkt | Status |
|---|---|
| **A1** — Theme per-mount overrides | ✅ done |
| **A2** — Multi-layout (Empty / Minimal / Main) | 🚧 partial (brakuje Dashboard.Public — nieużywane) |
| **A3** — Slot system w Website | ❎ skip (już mamy ekwiwalent w Zonit.Dashboard.Extensions; pivot na Website to oddzielny architektoniczny refaktor — odłożone) |
| **A4** — Persistence + zero flicker | ✅ done |
| **A5** — Connection UI | ✅ done (CSS + DOM shell nad domyślnym frameworkowym) |
| **A6** — Errors via Website pipeline | ✅ done |
| **B1** — Toast (MudBlazor adapter) | ✅ done |
| **B2** — Common primitives | ✅ done |
| **B3** — Error pages 401/403/500 | ✅ done |
| **B4** — Persistence | ✅ done (razem z A4) |
| **B5** — UserProfile slot | ✅ done |
| **B6** — Workspace switcher | ✅ done |
| **B7** — Project switcher | ✅ done |
| **B8** — Culture switcher | ✅ done |
| **B9** — TaskManager (toolbar + drawer + progress) | ✅ done (opt-in: `services.AddDashboardTaskManager()`) |
| **B10** — Responsive + swipe + RTL | ✅ done |
| **B11** — Connection UI razor | ✅ done |
| **B12** — NavItem/NavGroup props + Website error audit | ✅ done |

### Pliki utworzone / zmodyfikowane w tej sesji

**Website (extensions audit):**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website/Navigations/Models/NavItem.cs` — Badge / BadgeColor / Tooltip / Disabled + `NavBadgeColor` enum.
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website/Navigations/Models/NavGroup.cs` — analogicznie.

**Website.MudBlazor (reusable primitives):**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/_Imports.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/EmptyState.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/LoadingSpinner.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/PageHeader.razor`

**Zonit.Dashboard — Components:**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Layouts/DashboardEmptyLayout.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Layouts/RenderNavGroup.razor` — Badge/Tooltip/Disabled rendering
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Layouts/DashboardMainLayout.razor` — RTL provider, swipe area, progress slot
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Layouts/DashboardMainLayout.razor.cs` — viewport observer, swipe handler, IsRtl
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Common/UserProfileHeader.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Common/SocialMediaLinks.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Toolbar/UserProfileToolbar.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Toolbar/TaskManagerToolbar.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Drawers/CulturePanel.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Drawers/WorkspacePanel.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Drawers/ProjectPanel.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Drawers/TaskManagerPanel.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Connection.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/DashboardApp.razor` — `<meta color-scheme>` + zero-flicker inline script + `<Connection />`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/NotFound.razor` — `/error/404` route alias
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/Unauthorized.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/Forbidden.razor`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/ServerError.razor`

**Zonit.Dashboard — Extensions / DI:**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/UserProfileToolbarExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/CultureSwitcherDrawerExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/WorkspaceSwitcherDrawerExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/ProjectSwitcherDrawerExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/TaskManagerToolbarExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Extensions/Builtin/TaskManagerDrawerExtension.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/DependencyInjection/ServiceCollectionDashboardExtensions.cs` — Toast adapter, layout aliases, builtin extensions wiring
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/DependencyInjection/ServiceCollectionTaskManagerExtensions.cs` — opt-in `AddDashboardTaskManager()`

**Zonit.Dashboard — Theme + Per-mount + Persistence:**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Options/DashboardThemeOverrides.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Options/DashboardSiteOptions.cs` — `Theme { get; }` propagated do registry + branch middleware
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/DashboardMountRegistry.cs` — `MountSnapshot.ThemeOverrides`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/IDashboardCurrentSite.cs` — `ThemeOverrides` accessor
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Themes/Builtin/DefaultDashboardTheme.cs` — overrides layered nad tenant theme
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Themes/Services/ThemeManager.cs` — eager cookie read (SSR) + `SystemDarkCookieKey` + `ReadFromCookies()` helper

**Zonit.Dashboard — Services + csproj:**
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Services/ToastService.cs`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Zonit.Dashboard.csproj` — dodane PR/PkgRef do `Zonit.Extensions.Website.MudBlazor` i `Zonit.Messaging.Tasks.Abstractions`
- `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Directory.Packages.props` — MudBlazor 9.0.0-preview.2 → 9.4.0, dodany `Zonit.Messaging.Tasks.Abstractions`

### Co dalej (jedyny pozostały punkt)
- **A3** — push slot/hook system z `Zonit.Dashboard.Extensions` do `Zonit.Extensions.Website` jako generyczny `IWebsiteSlot` / `IWebsiteToolbar` / `IWebsiteDrawer`. Decyzja świadomie odłożona — aktualne `IToolbarExtension`/`IDrawerExtension` w Dashboard działają end-to-end. Pivot to oddzielny refaktor (~1 sesja) który zmieni tylko namespace + przeniesie pliki, nie zachowanie. Można zrobić, gdy pojawi się konsument non-dashboard chcący slotów (Documents host?).

### Smoke-test next
Następna sesja: uruchomić `Documents` host i zweryfikować w przeglądarce:
- Dashboard.Empty layout dostępny (`@attribute [LayoutKey("Dashboard.Empty")]`)
- Avatar w toolbar (anonimowy → Sign in, zalogowany → menu)
- Toggle drawerów (Theme, Culture, Workspace, Project) działa
- Dark/light cookie persistuje przez F5 bez flickeru
- Per-mount theme override (`o.Theme.PrimaryColor = "#ff0000"`) działa
- Reconnect modal styluje się w dashboard chrome (test: zatrzymać hosta na 30 s, wznowić)
- Error pages renderują się przy 401/403/404/500 + `/error/{code}`

---

## A. Decyzje architektoniczne (USTALONE z userem 2026‑05)

Te punkty wpływają na **wszystkie** następne — załatwić zanim ruszymy z UI.

### A1. Multi-dashboard + theme config — pułapka `Tenant.Settings.Theme`

**Problem**: aktualne `DefaultDashboardTheme` ciągnie kolory wprost z `ITenantProvider.Settings.Theme`.
To jest config **strony publicznej**. Można mieć N dashboardów na tym samym tenancie (admin / management / diagnostic)
i każdy z nich powinien móc mieć inny chrome (np. admin czerwona belka, diagnostic szare tło).

**Decyzja**:

- Wspólny baseline = `Tenant.Settings.Theme` (brand kolory tenanta).
- Per-mount override → `DashboardSiteOptions.Theme` (nowa property) — ustawiana w `app.UseDashboard("/admin", o => o.Theme.Primary = "#ff0000")`.
- `DashboardCurrentSite` już teraz nosi per-mount state (Layout/Whitelist/Snippet) — dorzucamy `Theme` do tego samego mechanizmu.
- `DashboardMainLayout` pyta `IDashboardCurrentSite.Theme` zanim spadnie do `Tenant.Settings.Theme`.

Status: ✅ **DONE**. Implementacja:
- `DashboardThemeOverrides` (`@Options/DashboardThemeOverrides.cs`) — 6 nullable color slotów.
- `DashboardSiteOptions.Theme { get; } = new()` — ustawiana w `app.UseDashboard("/admin", o => o.Theme.PrimaryColor = "#ff0000")`.
- `DashboardMountRegistry.MountSnapshot.ThemeOverrides` — propagated do registry.
- `IDashboardCurrentSite.ThemeOverrides` — per-mount accessor.
- `DefaultDashboardTheme` — layered: per-mount override > `Tenant.Settings.Theme`.

### A2. Multi-layout (Normal / Simple / Empty / …)

**Problem**: jeden `DashboardMainLayout` na wszystko. Stary projekt miał `MinimalLayout` i `PublicLayout`,
ale to było hardkodowane. Chcemy dynamiczny wybór layoutu per strona/plugin.

**Decyzja**:

- Layouty rejestrujemy w `ILayoutRegistry` (już istnieje w Website) pod stabilnymi kluczami:
  - `"Dashboard.Main"` — pełny chrome (appbar + nav drawer + ext drawers + breadcrumbs)
  - `"Dashboard.Minimal"` — chrome bez nav drawer, tylko appbar + content (login, onboarding, błędy)
  - `"Dashboard.Empty"` — czysty `@Body` w `MudThemeProvider` (dla pełnoekranowych widoków, np. crash screens, viewer-y)
  - `"Dashboard.Public"` — dla stron bez auth (lądowanie, marketing pod ścieżką dashboardu)
- Pluginy/strony wybierają via `@attribute [LayoutKey("Dashboard.Empty")]`.

Status: 🚧 **DONE (partial)**. Zarejestrowano w `ILayoutRegistry`:
- `"Dashboard.Main"` — już było
- `"Zonit.Minimal"` — już było (override frameworka)
- `"Dashboard.Minimal"` — ✅ dodane (alias z lepszą nazwą)
- `"Dashboard.Empty"` — ✅ dodane (`DashboardEmptyLayout.razor` — tylko providery + @Body)
- `"Dashboard.Public"` — ❌ todo (na razie nieużywane; dodać gdy potrzebne)

### A3. Slot / Hook system (modulacja dashboard z pluginów)

**Wizja usera**: zamiast hardkodować Workspace/Project/Culture switcher w dashboardzie,
plugin (`Zonit.Plugins.Organizations`, `Zonit.Plugins.Cultures`, …) **rejestruje swój slot/widget**,
a dashboard go pokazuje. **Tak żeby działało nie tylko w Dashboard, ale w każdym Website.**

**Decyzja**: aktualny `IToolbarExtension`/`IDrawerExtension` to jest dokładnie ten mechanizm,
ale w `Zonit.Dashboard.Extensions`. Trzeba go **wypchnąć do `Zonit.Extensions.Website`** jako generyczny
`IWebsiteSlot` / `IWebsiteToolbar` / `IWebsiteDrawer`, żeby:

1. Każdy area mógł kontrybuować nie tylko nawigację, ale też toolbar/drawer/footer/inline-slots.
2. Dashboard używa tej samej infrastruktury, plus dorzuca dashboard-specyficzne (anchor/width).
3. Nie-dashboardowe Website-y (np. publiczna strona) też mogą korzystać.

Status: ❌ todo. Duża zmiana, ale **odblokowuje punkty B5–B9** (zamiast pisać 4 specyficzne extensions
piszemy 4 generyczne slots).

### A4. Persistence (zero flicker przy ładowaniu)

**Problem**: stary Dashboard mial dark-theme flicker — ThemeProvider render się z domyślnym
trybem, potem JS odczytuje cookie, potem re-render. Widać białe mignięcie na ciemnym motywie.

**Decyzja** (do zaimplementowania):

1. SSR powinien znać tryb (Light/Dark/Auto) z **cookie** zanim wyśle pierwszy HTML.
   `IThemeManager` ma `HydrateAsync` (JS-based) — to za późno. Trzeba dodać `HydrateFromHttpContextAsync`
   wywoływane z `DashboardApp.razor` w fazie SSR.
2. Theme cookie czytamy w branch middleware (już mamy `branch.Use` w `OnConfigured`), stamp na
   `IThemeManager` przed render.
3. `prefers-color-scheme` (Auto mode) — pierwszy render w SSR zakłada Light; potem JS może
   przełączyć z `<meta name="color-scheme">` + CSS variable trick. Stosować inline script w
   `<head>` typu `if (window.matchMedia('(prefers-color-scheme: dark)').matches) document.documentElement.classList.add('mud-theme-dark')`
   — render bez flickeru, bo CSS przejmuje przed pierwszym paintem.
4. `PersistentComponentState` dla per-circuit state (theme mode, drawer states) — tak jak stary
   `ZonitDashboardExtension.cs` to próbował, ale bez bugów.

Status: ❌ todo.

### A5. Connection / offline UI (Blazor 10/11)

.NET 9 wprowadził lepsze reconnect UI dla Blazor Server (Persistent Components). .NET 10 ma
to dalej, .NET 11 obiecuje natywne offline tracking. Stary `Connection.razor` to był prymitywny
modal — chcemy:

1. Zacząć od **dotnet-default reconnect UI** stylizowanego przez MudBlazor (`MudSnackbar`).
2. Dodać `IConnectionState` (Online/Reconnecting/Offline) jako scoped service czytający
   `Blazor.defaultReconnectionHandler` przez JS interop.
3. UI w `DashboardApp.razor` (NIE w layoucie — musi być w głównym shellu żeby przeżyć layout-change).

Status: ❌ todo.

### A6. Errors — w Website czy w Dashboard?

Stary Dashboard miał `Pages/Errors/{401,403,404,500}.razor` — strony pod ścieżką
`/<dashboard>/401`. Nowy Dashboard ma tylko `NotFound.razor`.

**Pytanie**: czy `Zonit.Extensions.Website` ma natywny error pipeline (np. `UseExceptionHandler(...)` →
redirect do `/error/{code}`)? Trzeba sprawdzić `Source/Extensions/Zonit.Extensions/.../Website`.
Jeśli nie — dodajemy do Website (bo to wspólne dla wszystkich consumerów),
plus stylizowane strony per-host w Dashboard.

Status: ❌ todo (osobny audit Extensions.Website przy okazji).

---

## B. Plan migracji UI (po A1–A6)

Każdy punkt = jedna sesja (zwykle ~1h pracy). Po każdym: build = 0/0 + smoke test w Documents host.

### B1. Toast / Snackbar service ✅

- Stary: `Source/Zonit.Services.Dashboard/Services/ToastService.cs` (implementuje `IToastProvider` z Website).
- Nowy: brak rejestracji `IToastProvider` w `AddDashboard()`.
- **Zadanie**: skopiować `ToastService`, zarejestrować jako `services.TryAddScoped<IToastProvider, ToastService>()`.
- **Plik docelowy**: `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Services/ToastService.cs` — utworzony.
- **Rejestracja**: `services.RemoveAll<IToastProvider>() + AddScoped` w `AddDashboard()`.

### B2. Common UI primitives ✅

- Stary: `Areas/Dashboard/Components/Common/{EmptyState,LoadingSpinner,PageHeader,SocialMediaLinks,UserProfileHeader}.razor`.
- **Decyzja**: te 3 pierwsze są generyczne MudBlazor wrappery — przenieść do
  `Zonit.Extensions.Website.MudBlazor` (publiczne, dla wszystkich Areas).
  `SocialMediaLinks` i `UserProfileHeader` zostawić w `Zonit.Dashboard` (część chrome).
- **Pliki utworzone**:
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/EmptyState.razor`
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/LoadingSpinner.razor`
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website.MudBlazor/Components/PageHeader.razor`
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Common/UserProfileHeader.razor`
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Common/SocialMediaLinks.razor` (refaktoryzowane: lista linków parametrem, hard-coded /Discord/Facebook/Linkedin usunięte)
- **Decyzja**: 3 generyczne primitives bez `ICultureProvider` zależności — caller tłumaczy. `_Imports.razor` dodałem w `Zonit.Extensions.Website.MudBlazor` projekcie.

### B3. Error pages 401/403/500 + Dashboard.Empty layout ✅

- **Pliki utworzone**:
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/Unauthorized.razor` (`/401`, `/error/401`)
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/Forbidden.razor` (`/403`, `/error/403`)
  - `@/d:/GitVsCode/Zonit.Sdk/Source/Services/Zonit.Services.Dashboard/Source/Zonit.Dashboard/Components/Pages/Errors/ServerError.razor` (`/500`, `/error`, `/error/500`)
  - Plus `NotFound.razor` dostał dodatkowy `@page "/error/404"`.
- A6 confirmed: `SiteOptions.ExceptionHandlerPath = "/error"` (default) → `UseExceptionHandler + UseStatusCodePagesWithReExecute("/error/{0}")` — strony zostaną wywołane automatycznie.

### B4. Persistence + Theme cookie + zero-flicker ❌

- Zależy od **A4**.
- **Pliki dotknięte**:
  - `Source/Zonit.Dashboard/Themes/Services/ThemeManager.cs` — dorzucić `HydrateFromHttpContextAsync`.
  - `Source/Zonit.Dashboard/Components/DashboardApp.razor` — inline script + `<meta color-scheme>`.
  - `Source/Zonit.Dashboard/Options/DashboardSiteOptions.cs` — `OnConfigured` Use(branch) stamp theme.

### B5. UserProfile slot (toolbar + drawer-aside) ❌

- Zależy od **A3** (slot system). Jeśli A3 nie zrobione na czas — robimy jako `IToolbarExtension` w Dashboard.
- Stary: `Common/UserProfileHeader.razor` + `MainLayout.razor` linie 56–61.
- **Plik docelowy**: `Source/Zonit.Dashboard/Extensions/Builtin/UserProfileSlot.cs` (+ razor component).

### B6. Workspace (Organization) switcher ❌

- Stary: `Areas/Dashboard/Components/ChangeOrganization.razor` (6.3 kB).
- Zależność: `Zonit.Extensions.Organizations` (`IWorkspaceProvider`/`IWorkspaceManager`).
- **Decyzja**: zgodnie z A3 — to powinien być **plugin Organizations** który rejestruje slot,
  nie wbudowane w Dashboard. Plik źródłowy: `Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Organizations/Dashboard/WorkspaceSwitcherSlot.cs`.
- Tymczasowo (pre-A3): `Source/Zonit.Dashboard/Extensions/Builtin/WorkspaceSwitcherExtension.cs`.

### B7. Project (Catalog) switcher ❌

- Stary: `Areas/Dashboard/Components/ChangeProjects.razor` (6.2 kB).
- Analogicznie do B6, plugin: `Zonit.Extensions.Projects`.

### B8. Culture (language) switcher ❌

- Stary: `Source/Zonit.Services.Dashboard.Components/Cultures/{CultureButton,CultureDrawer}.razor`.
- Analogicznie do B6, plugin: `Zonit.Extensions.Cultures`.

### B9. TaskManager (long-running task tracker) ❌

- Stary:
  - `Areas/Dashboard/Components/TaskManager/TaskButton.razor` (4.4 kB) — toolbar widget z badge.
  - `Areas/Dashboard/Components/TaskManager/TaskDrawer.razor` (18.3 kB) — drawer z listą.
  - `MainLayout.razor.cs` linie 154–203 — auto progress-bar w app barze.
- Zależność: `Zonit.Messaging.Tasks` (`ITaskManager`/`TaskState`).
- **Plik docelowy**: `Source/Zonit.Dashboard/Extensions/Builtin/TaskManager{Toolbar,Drawer}Extension.cs`
  + komponenty razor. Progress bar w app barze → osobny `IAppbarSlot` (nowy slot do `DashboardMainLayout`).
- Aktywuje martwą opcję `DashboardLayoutOptions.ShowProgressBar`.

### B10. Responsywne drawery + Swipe + RTL ❌

- Stary: `MainLayout.razor.cs` używa `IBrowserViewportService` (linie 252–303) +
  `<MudSwipeArea>` w `MainLayout.razor` (linie 34, 115–145) + `<MudRTLProvider>` (linia 23).
- **Plik dotknięty**: `Source/Zonit.Dashboard/Components/Layouts/DashboardMainLayout.razor(.cs)`.
- Aktywuje martwe opcje: `EnableSwipeGestures`, `RightDrawerWidth`, breakpointy.

### B11. Connection / Reconnect UI ❌

- Zależy od **A5**.
- **Plik docelowy**: `Source/Zonit.Dashboard/Components/Connection.razor` + integracja w `DashboardApp.razor`.

### B12. Audit `Zonit.Extensions.Website` — Navigation + Errors ✅

- **Findings**:
  - `NavItem` / `NavGroup` miały: Title, Url, Icon (string — akceptuje SVG markup), Permission, Target, Order, Match, Children, Position, Expanded, Settings (dictionary).
  - **Brakowało**: Badge / BadgeColor / Tooltip / Disabled.
  - Error pipeline OK: `SiteOptions.ExceptionHandlerPath = "/error"` default + `UseStatusCodePagesWithReExecute("/error/{0}")` automatycznie.
- **Dodane**:
  - `NavItem.Badge / BadgeColor / Tooltip / Disabled` (`@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website/Navigations/Models/NavItem.cs`)
  - `NavGroup.Badge / BadgeColor / Tooltip / Disabled` (`@/d:/GitVsCode/Zonit.Sdk/Source/Extensions/Zonit.Extensions/Source/Zonit.Extensions.Website/Navigations/Models/NavGroup.cs`)
  - `NavBadgeColor` enum (Default/Primary/Secondary/Success/Warning/Error/Info) — obok `NavItem`.
- **RenderNavGroup zaktualizowany**: renderuje Badge (jako `MudChip`), Tooltip (jako `MudTooltip` wrapper), Disabled (przekazuje do `MudNavLink.Disabled` / `MudNavGroup.Disabled`).

---

## C. Już zmigrowane ✅ (referencja, nie do roboty)

| Funkcja | Stary | Nowy |
|---|---|---|
| `IDashboardExtension` / `IDrawerExtension` / `IToolbarExtension` | Abstractions/ | `Source/Zonit.Dashboard/Extensions/` |
| `IExtensionRegistry` | Application/Services/ | `Source/Zonit.Dashboard/Extensions/Services/ExtensionRegistry.cs` |
| Drawer state (`IExtensionDrawerStates`) | `IExtensionDrawerManager` | `Source/Zonit.Dashboard/Extensions/Services/ExtensionDrawerStates.cs` |
| `IDashboardTheme` + 3 built-in themes + `IThemeManager` | Application/Theme + Abstractions/Theme | `Source/Zonit.Dashboard/Themes/` |
| Główny layout | `Areas/Dashboard/Layouts/MainLayout.razor` | `Source/Zonit.Dashboard/Components/Layouts/DashboardMainLayout.razor` |
| Minimal layout | `Components/Layouts/MinimalLayout.razor` | `Source/Zonit.Dashboard/Components/Layouts/DashboardMinimalLayout.razor` |
| Theme selector | inline w MainLayout (9.5 kB) | `Source/Zonit.Dashboard/Extensions/Builtin/ThemeSelectorDrawerExtension.cs` + Panel |
| Mount entry | `UseDashboardServices<T>` | `app.UseDashboard()` → `UseWebsite<DashboardApp, DashboardSiteOptions>` |
| `DashboardSettings` → split | `Abstractions/DashboardSettings.cs` | `SiteOptions` (base) + `DashboardSiteOptions` + `Tenant.Settings` |
| Per-mount state survival HTTP↔circuit | `ZonitDashboardExtension.cs` (PCS hack) | `DashboardMountRegistry` + `IDashboardCurrentSite` |
| 404 page | `Pages/Errors/404.razor` | `Components/Pages/NotFound.razor` |

## D. Świadomie pominięte ❎

| Funkcja | Powód |
|---|---|
| `INavigationManager` + `NavigationItem` jako dashboard-private API | Zastąpione `IWebsiteArea.Navigation` → `INavigationProvider` z Website (single source of truth). Patrz B12 — weryfikacja kompletności propów. |
| `ISettingsExtension` | Per-tenant settings via `Zonit.Extensions.Tenants`, per-user settings to zwykłe razor pages. |
| `ISettingsManager` / `SettingsRepository` / `ZonitDashboardExtension.cs` | Zastąpione `IDashboardCurrentSite` + `DashboardMountRegistry`. |
| `IExtensionManager.Drawer(name)` fluent API | Nowe API: `IExtensionDrawerStates.GetState(id)`. Przy migracji punktów B5–B9 używamy nowego. |
| `DashboardSettings.Theme.*` (Primary/Secondary/…) | Per-tenant brand → `Tenant.Settings.Theme`. Per-mount override → A1. |
| `Components/Layouts/PublicLayout.razor` | Zastąpione przez `Dashboard.Empty` z A2 (jeśli potrzebne). |

---

## E. Kolejność wykonania (rekomendowana)

1. **A1 + A2** razem (theme per mount + multi-layout) — fundament dla wszystkiego.
2. **A4** (persistence + zero flicker) — naprawia widoczny bug starego Dashboardu.
3. **B1** (Toast) — szybki win, mała ilość kodu.
4. **B2** (Common primitives) — odblokowuje pluginy.
5. **B12** (audit Website nav + errors) — pre-req dla B3 i pluginowych slotów.
6. **A6 + B3** (errors).
7. **A3** (slot system w Website) — jeśli OK na czas; inaczej B5–B8 jako Dashboard-private extensions.
8. **B5 → B6 → B7 → B8 → B9** w tej kolejności (od najprostszego).
9. **B10** (responsywne + swipe + RTL).
10. **A5 + B11** (connection UI).

---

## F. Source files cleanup

Po pełnej migracji (wszystkie B✅), do **usunięcia ze drzewa**:

- `Source/Zonit.Services.Dashboard/`
- `Source/Zonit.Services.Dashboard.Abstractions/`
- `Source/Zonit.Services.Dashboard.Application/`
- `Source/Zonit.Services.Dashboard.Components/`

Najpierw archiwizacja na GitHub (zgodnie z konwencją Zonit dla legacy repo).
