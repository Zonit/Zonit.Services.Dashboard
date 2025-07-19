using Example.Presentation.Manager.Pages;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Navigations.Types;

namespace Example.Presentation.Manager.Data;

internal class NavData(INavigationProvider _navigation) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _navigation.Add(new()
        {
            Title = "Home",
            Area = AreaType.Manager,
            Order = 10,
            Icon = Icons.Material.Filled.Home,
            Link = new("Home", "home"),
        });

        _navigation.Add(new()
        {
            Title = "Counter",
            Area = AreaType.Manager,
            Order = 20,
            Icon = Icons.Material.Filled.ExposurePlus1,
            Link = new("Counter", Counter.Route),
        });

        _navigation.Add(new()
        {
            Title = "Test task",
            Area = AreaType.Manager,
            Order = 21,
            Icon = Icons.Material.Filled.ExposurePlus1,
            Link = new("TestTask", TestTask.Route),
        });

        _navigation.Add(new()
        {
            Title = "Komponenty MudBlazor",
            Area = AreaType.Manager,
            Order = 30,
            Icon = Icons.Material.Filled.ViewList,
            Expanded = true,
            Children = [
                new()
                {
                    Title = "Formularze",
                    Url = "form-components",
                    Order = 10,
                    Icon = Icons.Material.Filled.Description
                },
                new()
                {
                    Title = "Wyświetlanie danych",
                    Url = "display-components",
                    Order = 20,
                    Icon = Icons.Material.Filled.TableChart
                },
                new()
                {
                    Title = "Nawigacja",
                    Url = "navigation-components",
                    Order = 30,
                    Icon = Icons.Material.Filled.Menu
                },
                new()
                {
                    Title = "Powiadomienia",
                    Url = "feedback-components",
                    Order = 40,
                    Icon = Icons.Material.Filled.Notifications
                },
                new()
                {
                    Title = "Przyciski i ikony",
                    Url = "button-components",
                    Order = 50,
                    Icon = Icons.Material.Filled.TouchApp
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
