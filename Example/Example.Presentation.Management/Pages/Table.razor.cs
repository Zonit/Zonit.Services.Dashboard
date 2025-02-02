using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Zonit.SDK.Website;

namespace Example.Presentation.Management.Pages;

[Authorize]
[Route("/" + Route)]
public sealed partial class Table : PageComponent, IAreaManagement // IAreaWeb, IAreaManager, IAreaManagement, IAreaDiagnostics
{
    public const string Route = "Table";

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}