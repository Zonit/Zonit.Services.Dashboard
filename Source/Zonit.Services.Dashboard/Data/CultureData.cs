using Zonit.Extensions.Cultures.Models;
using Zonit.Extensions.Cultures;

namespace Zonit.Services.Dashboard.Data;

internal class CultureData(ITranslationManager _translationRepository) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var variable = new List<Variable>()
        {
            // Unauthorized
            new("Unauthorized", [
                new() { Culture = "en-us", Content = "Unauthorized"},
                new() { Culture = "pl-pl", Content = "Nieautoryzowany"},
            ]),
            new("You are not authorized to access this resource.", [
                new() { Culture = "en-us", Content = "You are not authorized to access this resource."},
                new() { Culture = "pl-pl", Content = "Nie masz uprawnień dostępu do tego zasobu."},
            ]),

            // Forbidden
            new("Forbidden", [
                new() { Culture = "en-us", Content = "Forbidden"},
                new() { Culture = "pl-pl", Content = "Zakazane"},
            ]),
            new("Access to this resource is forbidden.", [
                new() { Culture = "en-us", Content = "Access to this resource is forbidden."},
                new() { Culture = "pl-pl", Content = "Dostęp do tego zasobu jest zabroniony."},
            ]),
            
            // Not Found
            new("Not Found", [
                new() { Culture = "en-us", Content = "Not Found"},
                new() { Culture = "pl-pl", Content = "Nie znaleziono"},
            ]),
            new("The requested resource could not be found.", [
                new() { Culture = "en-us", Content = "The requested resource could not be found."},
                new() { Culture = "pl-pl", Content = "Nie można znaleźć żądanego zasobu."},
            ]),

            // Internal Server Error
            new("Internal Server Error", [
                new() { Culture = "en-us", Content = "Internal Server Error"},
                new() { Culture = "pl-pl", Content = "Wewnętrzny błąd serwera"},
            ]),
            new("There was an internal server error.", [
                new() { Culture = "en-us", Content = "There was an internal server error."},
                new() { Culture = "pl-pl", Content = "Wystąpił wewnętrzny błąd serwera."},
            ]),


            new("Back to previous page", [
                new() { Culture = "en-us", Content = "Back to previous page"},
                new() { Culture = "pl-pl", Content = "Powrót do poprzedniej strony"},
            ]),
        };

        _translationRepository.AddRange(variable);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}