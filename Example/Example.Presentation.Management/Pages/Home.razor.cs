using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Zonit.SDK.Website;

namespace Example.Presentation.Management.Pages;

[Authorize]
[Route("/")]
[Route("/" + Route)]
public sealed partial class Home : PageBase, IAreaManagement
{
    public const string Route = "home";

    protected override List<BreadcrumbsModel>? Breadcrumbs =>
    [
        new("Home", "Home", true),
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