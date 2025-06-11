# Zonit.Services.Dashboard

## Overview

**Zonit Dashboard** is an advanced solution for building modern, modular, and flexible administration panels using the Blazor framework. It enables the creation of interactive web applications with a rich user interface based on the [MudBlazor](https://mudblazor.com/) component library.

The dashboard is designed with scalability and flexibility in mind—it can be duplicated to provide multiple layered panels (such as user, admin, or manager dashboards) within a single project, each accessible from different URL addresses depending on middleware and configuration. The project is fully modular, supports dynamic extensions and modeling, and comes with comprehensive Zonit.Extensions out-of-the-box.

## Features

- **Modular Architecture**  
  Easily extend functionality through a powerful extensions system (e.g., for identity, projects, organizations, finances, tasks, and more).

- **Advanced Customization**  
  Configurable themes with full support for both light and dark modes.

- **Organization Management**  
  Built-in mechanisms for managing users and organizational structures.

- **Project & Task Management**  
  Modules to facilitate project tracking and task management.

- **Authentication & Authorization**  
  Comprehensive access control with integrated role management.

- **Multi-Culture Support**  
  Complete internationalization support for global applications.

- **Modern UI/UX**  
  Responsive design powered by MudBlazor components.

## Technologies Used

- **[Blazor Server](https://docs.microsoft.com/en-us/aspnet/core/blazor/)** — Interactive web UI framework
- **[MudBlazor](https://mudblazor.com/)** — Material design components for Blazor
- **Zonit.Extensions:**  
  - `Identity` — Identity and authentication management  
  - `Cultures` — Multi-culture and localization support  
  - `Projects` — Project management tools  
  - `Organizations` — Organization administration  
  - `Wallets` — Finance and wallet management  
  - `Task` — Task and to-do modules

## Quick Start

Here's an example of how to configure the dashboard with different areas in your `.NET` application:

```csharp
// Register dashboard services
builder.Services.AddDashboardService();

// Configure Manager Area
app.UseDashboardServices<IAreaManager>(new DashboardOptions
{
    Directory = "Manager",
    Title = "Manager",
    Extensions = [ Extensions.Identity, Extensions.Cultures, Extensions.Projects, Extensions.Organizations ]
});

// Configure Management Area
app.UseDashboardServices<IAreaManagement>(new DashboardOptions
{
    Directory = "management",
    Title = "Management"
});
```
> **Note:** The dashboard architecture supports multiple independent panels (e.g., user, admin, management) under different URLs, all within a single project instance.

## Out-of-the-Box Functionality

- Turnkey integration—works instantly with all included Zonit.Extensions
- Ready for modeling and customization per panel/layer
- Scalable to any number of dashboards as required by your middleware configuration

## Examples

Find comprehensive usage examples in the `example` directory for inspiration and typical setups.
