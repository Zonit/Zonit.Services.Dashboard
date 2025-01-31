using Example.Presentation.Management.Pages;
using Microsoft.Extensions.Hosting;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Navigations.Types;

namespace Example.Presentation.Management.Data;

internal class NavData(INavigationProvider _navigation) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _navigation.Add(new()
        {
            Title = "Home",
            Area = AreaType.Management,
            Order = 60,
            Icon = IconData.GiftIcon,
            Link = new("Home", Home.Route),
        });
        _navigation.Add(new()
        {
            Title = "Counter",
            Area = AreaType.Management,
            Order = 60,
            Icon = IconData.GiftIcon,
            Link = new("Counter", Counter.Route),
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}