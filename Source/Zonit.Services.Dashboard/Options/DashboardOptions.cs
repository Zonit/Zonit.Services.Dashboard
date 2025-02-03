using Zonit.Extensions.Website.Abstractions.Navigations.Types;

namespace Zonit.Services.Dashboard.Options;

public class DashboardOptions
{
    public string Directory { get; set; } = "Dashboard";
    public string Title { get; set; } = "Dashboard";



    public string? Permission { get; set; }

    public bool EnableAntiforgery { get; init; } = true;




    public string ThemeColor { get; set; } = "#ff6100";
    public string FavIcon { get; set; } = "favicon.svg";
}
