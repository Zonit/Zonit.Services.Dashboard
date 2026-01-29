using Example.Presentation.Manager.Pages;
using Example.Presentation.Manager.Pages.Examples;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Navigations.Types;

namespace Example.Presentation.Manager.Data;

internal class NavData(INavigationProvider _navigation) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Główne pojedyncze linki
        _navigation.Add(new()
        {
            Title = "Dashboard",
            Area = AreaType.Manager,
            Order = 5,
            Icon = Icons.Material.Filled.Dashboard,
            Link = new("Home", "home"),
        });

        // Example Pages - różne layouty dashboardu
        _navigation.Add(new()
        {
            Title = "Examples",
            Area = AreaType.Manager,
            Order = 6,
            Icon = Icons.Material.Filled.Widgets,
            Children = [
                new()
                {
                    Title = "All Examples",
                    Url = "examples",
                    Order = 1,
                    Icon = Icons.Material.Filled.Apps
                },
                new()
                {
                    Title = "Dashboard",
                    Url = Dashboard.Route,
                    Order = 10,
                    Icon = Icons.Material.Filled.Dashboard
                },
                new()
                {
                    Title = "Users List",
                    Url = UsersList.Route,
                    Order = 20,
                    Icon = Icons.Material.Filled.People
                },
                new()
                {
                    Title = "User Form",
                    Url = UserForm.Route,
                    Order = 30,
                    Icon = Icons.Material.Filled.PersonAdd
                },
                new()
                {
                    Title = "Profile",
                    Url = Profile.Route,
                    Order = 40,
                    Icon = Icons.Material.Filled.AccountCircle
                },
                new()
                {
                    Title = "Settings",
                    Url = Settings.Route,
                    Order = 50,
                    Icon = Icons.Material.Filled.Settings
                },
                new()
                {
                    Title = "Error Pages",
                    Url = Error404.Route,
                    Order = 60,
                    Icon = Icons.Material.Filled.ErrorOutline
                }
            ]
        });

        // Grupa Content
        _navigation.Add(new()
        {
            Title = "Content",
            Area = AreaType.Manager,
            Order = 10,
            Children = [
                new()
                {
                    Title = "Create Content",
                    Url = "form-components",
                    Order = 10,
                    Icon = Icons.Material.Filled.Edit
                },
                new()
                {
                    Title = "Generated Content",
                    Url = "display-components",
                    Order = 20,
                    Icon = Icons.Material.Filled.AutoAwesome
                },
                new()
                {
                    Title = "Drafts",
                    Url = "navigation-components",
                    Order = 30,
                    Icon = Icons.Material.Filled.Drafts
                },
                new()
                {
                    Title = "Saved Presets",
                    Url = "feedback-components",
                    Order = 40,
                    Icon = Icons.Material.Filled.Bookmark
                }
            ]
        });

        // Grupa Team
        _navigation.Add(new()
        {
            Title = "Team",
            Area = AreaType.Manager,
            Order = 20,
            Children = [
                new()
                {
                    Title = "Members",
                    Url = Counter.Route,
                    Order = 10,
                    Icon = Icons.Material.Filled.People
                },
                new()
                {
                    Title = "API Keys",
                    Url = "button-components",
                    Order = 20,
                    Icon = Icons.Material.Filled.Key
                },
                new()
                {
                    Title = "Integrations",
                    Url = TestTask.Route,
                    Order = 30,
                    Icon = Icons.Material.Filled.Extension
                },
                new()
                {
                    Title = "Billing",
                    Url = "billing",
                    Order = 40,
                    Icon = Icons.Material.Filled.Receipt
                },
                new()
                {
                    Title = "Team Profile",
                    Url = "team-profile",
                    Order = 50,
                    Icon = Icons.Material.Filled.Settings
                }
            ]
        });

        // Grupa Other
        _navigation.Add(new()
        {
            Title = "Other",
            Area = AreaType.Manager,
            Order = 30,
            Children = [
                new()
                {
                    Title = "Marketplace",
                    Url = "marketplace",
                    Order = 10,
                    Icon = Icons.Material.Filled.Store
                },
                new()
                {
                    Title = "Learn",
                    Url = "learn",
                    Order = 20,
                    Icon = Icons.Material.Filled.School
                },
                new()
                {
                    Title = "Quick Start",
                    Url = "quick-start",
                    Order = 30,
                    Icon = Icons.Material.Filled.FlashOn
                },
                new()
                {
                    Title = "Show Tour Guide",
                    Url = "tour",
                    Order = 40,
                    Icon = Icons.Material.Filled.Tour
                },
                new()
                {
                    Title = "Leave a Review",
                    Url = "review",
                    Order = 50,
                    Icon = Icons.Material.Filled.Star
                }
            ]
        });

        _navigation.Add(new()
        {
            Title = "Management",
            Area = AreaType.Manager,
            Position = "right",
            Order = int.MaxValue,
            Icon = IconData.GiftIcon,
            Link = new("Management", "/management"),
            Permission = "AllowManagement"
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
