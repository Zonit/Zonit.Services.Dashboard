# Zonit.Services.Dashboard

A modern, modular administration panel framework for Blazor with **full AOT/Trimming support**.

---

## :package: NuGet Packages

| Package | Description |
|---------|-------------|
| **Zonit.Services.Dashboard** | Main dashboard implementation |
| **Zonit.Services.Dashboard.Abstractions** | Interfaces and base classes |
| **Zonit.Services.Dashboard.Application** | Application layer services |
| **Zonit.Services.Dashboard.Components** | Shared Blazor components |

---

## Features

- **Multi-Area Architecture** — Create multiple independent dashboards (user, admin, management) under different URLs
- **Extension System** — Modular, pluggable extensions for drawers, toolbars, navigation, and settings
- **AOT/Trimming Safe** — No runtime reflection, uses `RenderFragment` factories
- **MudBlazor UI** — Material Design components with dark/light theme support
- **Built-in Extensions** — Identity, Cultures, Projects, Organizations, Tasks
- **Responsive Layout** — Desktop and mobile-optimized

---

## Requirements

- .NET 8, .NET 9 or .NET 10
- MudBlazor 8+

---

## Quick Start

### 1. Install Package

```powershell
dotnet add package Zonit.Services.Dashboard
```

### 2. Register Services

```csharp
// Program.cs
builder.Services.AddDashboardService();
```

### 3. Configure Areas

```csharp
// Configure Manager Area
app.UseDashboardServices<IAreaManager>(new DashboardOptions
{
    Directory = "Manager",
    Title = "Manager",
    Extensions = [ Extensions.Identity, Extensions.Cultures, Extensions.Projects ]
});

// Configure Admin Area (with permission requirement)
app.UseDashboardServices<IAreaManagement>(new DashboardOptions
{
    Directory = "management",
    Title = "Management",
    Permission = "AllowManagement",
    Extensions = [ Extensions.Identity, Extensions.Cultures ]
});
```

### 4. Define Area Marker

```csharp
// IAreaManager.cs
public interface IAreaManager : IDashboardArea { }
```

---

## DashboardOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Directory` | `string` | `"dashboard"` | URL path segment (e.g., `/manager`) |
| `Title` | `string` | `"Dashboard"` | Dashboard title displayed in UI |
| `Permission` | `string?` | `null` | Required authorization policy |
| `Extensions` | `string[]` | `[]` | Enabled extension identifiers |
| `ThemeColor` | `string` | `"#ff6100"` | Primary theme color |
| `FavIcon` | `string` | `"favicon.svg"` | Favicon path |
| `EnableAntiforgery` | `bool` | `true` | Enable CSRF protection |
| `CustomSnippet` | `string?` | `null` | Custom HTML/JS snippet |

---

## Extension System

Dashboard uses a modular extension system that is **fully AOT-safe**. Extensions provide:

- **Navigation** — Menu items
- **Drawers** — Slide-out panels
- **Toolbar** — AppBar buttons
- **Settings** — Configuration panels

### Built-in Extensions

| Extension | ID | Description |
|-----------|-----|-------------|
| Identity | `Extensions.Identity` | User profile and authentication |
| Cultures | `Extensions.Cultures` | Language/culture selector |
| Projects | `Extensions.Projects` | Project management |
| Organizations | `Extensions.Organizations` | Organization management |
| Tasks | `Extensions.Task` | Background task monitoring |
| Wallets | `Extensions.Wallets` | Finance/wallet module |

### Creating Custom Extensions

#### Drawer Extension (AOT-Safe)

```csharp
using Zonit.Services.Dashboard.Abstractions;

public class NotificationDrawerExtension : DrawerExtensionBase<NotificationDrawer>
{
    public override string Id => "notifications";
    public override string Name => "Notifications";
    public override int Order => 100;
    public override DrawerAnchor Anchor => DrawerAnchor.End;
    public override int Width => 400;
}
```

#### Register Extension

```csharp
// In DI configuration - uses factory delegate (AOT-safe)
services.AddDrawerExtension<NotificationDrawerExtension>(
    _ => new NotificationDrawerExtension()
);
```

### Extension Interfaces

| Interface | Purpose |
|-----------|---------|
| `IDrawerExtension` | Slide-out drawer panel |
| `IToolbarExtension` | AppBar toolbar items |
| `INavigationExtension` | Navigation menu items |
| `ISettingsExtension` | Settings panel sections |

### Base Classes (AOT-Safe)

| Base Class | For |
|------------|-----|
| `DrawerExtensionBase<TComponent>` | Drawer extensions with typed component |
| `ToolbarExtensionBase<TComponent>` | Toolbar extensions |
| `SettingsExtensionBase<TComponent>` | Settings extensions |
| `NavigationExtensionBase` | Navigation extensions |

---

## Managing Drawers

```csharp
@inject IExtensionDrawerManager DrawerManager

@code {
    private void OpenNotifications()
    {
        var drawer = DrawerManager.GetDrawer("notifications");
        drawer.Open();
    }
    
    private void ToggleCultures()
    {
        var drawer = DrawerManager.GetDrawer("cultures");
        drawer.Toggle();
    }
}
```

### IExtensionDrawer API

| Method | Description |
|--------|-------------|
| `Open()` | Opens the drawer |
| `Close()` | Closes the drawer |
| `Toggle()` | Toggles drawer state, returns new state |
| `SetOpen(bool)` | Sets drawer state explicitly |
| `IsOpen` | Current drawer state |
| `OnChange` | Event when state changes |

---

## Navigation Provider

Provide navigation items for the dashboard sidebar:

```csharp
public class MyNavigation : INavigationProvider
{
    public IEnumerable<NavigationItem> GetNavigation()
    {
        yield return new NavigationItem
        {
            Title = "Dashboard",
            Url = "/manager",
            Icon = Icons.Material.Filled.Dashboard,
            Order = 1
        };
        
        yield return new NavigationItem
        {
            Title = "Users",
            Url = "/manager/users",
            Icon = Icons.Material.Filled.People,
            Order = 2,
            Children = [
                new() { Title = "List", Url = "/manager/users", Order = 1 },
                new() { Title = "Add", Url = "/manager/users/add", Order = 2 }
            ]
        };
    }
}
```

Register in DI:

```csharp
services.AddScoped<INavigationProvider, MyNavigation>();
```

---

## Project Structure

```
Source/
├── Zonit.Services.Dashboard/              # Main implementation
│   ├── Areas/Dashboard/
│   │   ├── Layouts/                       # MainLayout.razor
│   │   └── Components/                    # Navbar, Sidebar, etc.
│   └── DependencyInjection/               # Service registration
├── Zonit.Services.Dashboard.Abstractions/ # Interfaces & types
│   ├── IDashboardExtension.cs             # Extension interfaces
│   ├── ExtensionBases.cs                  # Base classes (AOT-safe)
│   ├── IExtensionDrawerManager.cs         # Drawer management
│   └── IExtensionRegistry.cs              # Extension registry
├── Zonit.Services.Dashboard.Application/  # Application services
└── Zonit.Services.Dashboard.Components/   # Shared components
```

---

## Example Project

See the `Example/` directory for a complete working example including:

- Multiple dashboard areas (Manager, Management, Diagnostic)
- Custom navigation provider
- Task management integration
- Role-based authorization

### Running the Example

```powershell
cd Example/Example
dotnet run
```

Navigate to:
- `/manager` — Manager dashboard
- `/management` — Management dashboard (requires "Worker" role)
- `/diagnostic` — Diagnostic dashboard (requires "Developer" role)

---

## AOT/Trimming Compatibility

This library is designed for full Native AOT and trimming support:

:white_check_mark: **No reflection** — Uses `RenderFragment` factories  
:white_check_mark: **No `Activator.CreateInstance`** — Uses DI factory delegates  
:white_check_mark: **No `[DynamicallyAccessedMembers]`** — Type-safe generics  
:white_check_mark: **Source Generator ready** — For extension auto-discovery

---

## Contributing & Support

Found a bug or have a feature request? Open an [issue](https://github.com/Zonit/Zonit.Services.Dashboard/issues/new) on GitHub!

---

## License

[MIT](LICENSE)
