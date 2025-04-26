using Microsoft.AspNetCore.Components;
using Zonit.Services.Dashboard;
using Zonit.Services.Dashboard.Repositories;

namespace Zonit.Extensions;

public sealed class ZonitDashboardExtension : ComponentBase, IDisposable
{
    [Inject]
    ISettingsManager Settings { get; set; } = default!;

    [Inject]
    PersistentComponentState ApplicationState { get; set; } = default!;

    PersistingComponentStateSubscription persistingSubscription;

    TransferModel Transfer { get; set; } = null!;

    public class TransferModel
    {
        public DashboardOptions? DashboardOptions { get; set; }
        public string? Area { get; set; }
    }

    protected override void OnInitialized()
    {
        persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

        if (!ApplicationState.TryTakeFromJson<TransferModel>("ZonitDashboardExtension", out var restored))
        {
            Transfer = new TransferModel { 
                DashboardOptions = Settings.Settings,
                Area = Settings.Area.GetType().AssemblyQualifiedName
            };
        }
        else
        {
            Transfer = restored!;
        }
            
        if(Transfer.DashboardOptions is not null)
            Settings.SetSettings(Transfer.DashboardOptions);

        if (Transfer.Area is not null)
        {
            var areaType = Type.GetType(Transfer.Area);
            if (areaType is not null && areaType.IsInterface)
                Settings.SetArea(areaType);
        }

    }

    private Task PersistData()
    {
        ApplicationState.PersistAsJson("ZonitDashboardExtension", Transfer);

        return Task.CompletedTask;
    }

    public void Dispose()
        => persistingSubscription.Dispose();
}