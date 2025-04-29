using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Zonit.Services.Dashboard.Areas.Dashboard;

public partial class Routes : ComponentBase
{
    [Parameter]
    public string[] AssemblyNames { get; init; } = [];

    public IEnumerable<Assembly> Assemblies { get; private set; } = [];

    [Inject]
    private ILogger<Routes> Logger { get; set; } = default!;

    protected override void OnInitialized()
    {
        Assemblies = AssemblyNames
            .Select(LoadAssembly)
            .Where(a => a is not null)!
            .Cast<Assembly>();
    }

    private Assembly? LoadAssembly(string assemblyName)
    {
        try
        {
            return Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading assembly '{AssemblyName}'", assemblyName);
            return null;
        }
    }
}
