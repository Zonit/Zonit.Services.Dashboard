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

    private PersistingComponentStateSubscription _persistingSubscription;
    private bool _disposed = false;

    private TransferModel Transfer { get; set; } = new();

    public class TransferModel
    {
        public DashboardOptions? DashboardOptions { get; set; }
        public string? Area { get; set; }
    }

    protected override void OnInitialized()
    {
        if (_disposed) return;

        try
        {
            // Sprawdź, czy wszystkie wymagane serwisy są dostępne
            if (Settings is null)
            {
                throw new InvalidOperationException("ISettingsManager was not injected. Check DI registration.");
            }

            if (ApplicationState is null)
            {
                throw new InvalidOperationException("PersistentComponentState was not injected. Check DI registration.");
            }

            // Zarejestruj subskrypcję tylko jeśli komponent nie został jeszcze usunięty
            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

            // Spróbuj przywrócić dane z persystentnego stanu
            if (!ApplicationState.TryTakeFromJson<TransferModel>("ZonitDashboardExtension", out var restored))
            {
                // Jeśli nie ma zapisanych danych, utwórz nowy model transferu
                Transfer = new TransferModel();
                
                // Bezpiecznie pobierz ustawienia, jeśli są dostępne
                try
                {
                    if (Settings.Settings is not null)
                    {
                        Transfer.DashboardOptions = Settings.Settings;
                    }
                }
                catch (Exception)
                {
                    // Settings może nie być jeszcze dostępne - ignoruj
                }
            }
            else
            {
                Transfer = restored ?? new TransferModel();
            }

            // Zastosuj przywrócone ustawienia tylko jeśli są dostępne i komponent nie został usunięty
            if (!_disposed && Transfer.DashboardOptions is not null)
            {
                try
                {
                    Settings.SetSettings(Transfer.DashboardOptions);
                }
                catch (Exception)
                {
                    // Ignoruj błędy ustawień w przypadku problemów z DI
                }
            }
        }
        catch (Exception)
        {
            // W przypadku jakichkolwiek błędów podczas inicjalizacji, po prostu utwórz pusty model
            Transfer = new TransferModel();
        }
    }

    private Task PersistData()
    {
        // Sprawdź, czy komponent nie został usunięty przed persystowaniem
        if (_disposed || ApplicationState is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            // Aktualizuj Transfer z bieżącymi ustawieniami przed persystowaniem
            if (Settings?.Settings is not null)
            {
                Transfer.DashboardOptions = Settings.Settings;
            }

            ApplicationState.PersistAsJson("ZonitDashboardExtension", Transfer);
        }
        catch (ObjectDisposedException)
        {
            // ApplicationState został usunięty - zignoruj
        }
        catch (Exception)
        {
            // Ignoruj inne błędy persystowania
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            _persistingSubscription.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Subskrypcja została już usunięta - zignoruj
        }
        catch (Exception)
        {
            // Ignoruj inne błędy dispose
        }
    }
}