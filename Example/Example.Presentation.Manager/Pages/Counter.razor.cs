using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;

namespace Example.Presentation.Manager.Pages;

[Authorize]
[Route("/" + Route)]
public sealed partial class Counter : PageBase, IAreaManager
{
    public const string Route = "counter";

    protected override List<BreadcrumbsModel>? Breadcrumbs =>
    [
        new("Home", "Home"),
        new WorkspaceBreadcrumbs(),
        new CatalogBreadcrumbs(),
        new("Counter", "Counter"),
    ];

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