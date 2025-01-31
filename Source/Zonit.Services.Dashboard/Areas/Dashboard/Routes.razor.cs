using Microsoft.AspNetCore.Components;
using System.Reflection;
using Zonit.Extensions.Website;
using Zonit.Services.Dashboard.Services;

namespace Zonit.Services.Dashboard.Areas.Dashboard;

public partial class Routes : ComponentBase
{
    public IEnumerable<Assembly> _assemblies;

    protected override void OnInitialized()
    {
        var t = new AssemblyScannerService<IAreaManager>();
        _assemblies = t.Types();

    }
}
