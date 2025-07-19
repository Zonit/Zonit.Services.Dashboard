using Microsoft.AspNetCore.Components;
using System.Reflection;
using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.Dashboard.Services;

namespace Zonit.Services.Dashboard.Areas.Dashboard;

public partial class App : ComponentBase
{
    [Inject]
    ISettingsProvider SettingsProvider { get; set; } = default!;

    [Inject]
    ISettingsManager SettingsManager { get; set; } = default!;

    public string[] _assemblyNames = [];

    protected override void OnInitialized()
    {
        Type areaManagerType = SettingsManager.Area;

        // Tworzymy typ generyczny AssemblyScannerService<IAreaManager>
        Type scannerType = typeof(AssemblyScannerService<>).MakeGenericType(areaManagerType);

        // Tworzymy instancję AssemblyScannerService<IAreaManager>
        object? scannerInstance = Activator.CreateInstance(scannerType);

        if (scannerInstance != null)
        {
            // Wywołujemy metodę 'Types' na dynamicznie utworzonym obiekcie
            var method = scannerType.GetMethod("Types"); // Pobieramy metodę 'Types'
            var result = method?.Invoke(scannerInstance, null); // Wywołujemy metodę na instancji

            // Jeśli metoda Types() zwraca IEnumerable<Assembly>, możemy to rzutować:
            if (result is IEnumerable<Assembly> assemblies)
            {
                // Możesz teraz uzyskać dane z assemblies
                _assemblyNames = assemblies.Select(a => a.FullName ?? a.GetName().Name ?? string.Empty).ToArray();
            }
        }
    }
}
