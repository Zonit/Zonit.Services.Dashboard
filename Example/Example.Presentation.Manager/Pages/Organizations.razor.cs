using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Microsoft.AspNetCore.Authorization;

namespace Example.Presentation.Manager.Pages;

[Route("/" + Route), Authorize]
public sealed partial class Organizations : PageBase, IAreaManager
{
    public const string Route = "organizations";

    protected override List<BreadcrumbsModel>? Breadcrumbs =>
    [
        new("Home", "Home"),
        new WorkspaceBreadcrumbs(),
        new("Details", "#", true),
        new("Details", "#", true),
        new("Details", "#", true),
        new("Details", "#", true),
        new("Details", "#", true),
    ];

}