﻿using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Microsoft.AspNetCore.Authorization;
using Zonit.SDK.Website;

namespace Example.Presentation.Manager.Pages;

[Route("/" + Route), Authorize]
public sealed partial class Organizations : PageComponent, IAreaManager
{
    public const string Route = "Organizations";

    protected override List<BreadcrumbsModel>? Breadcrumbs =>
    [
        new("Home", "Home"),
        new WorkspaceBreadcrumbs(),
        new("Details", "#", true),
    ];

}