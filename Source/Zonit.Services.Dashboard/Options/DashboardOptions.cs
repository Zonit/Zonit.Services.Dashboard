namespace Zonit.Services.Dashboard.Options;

public class DashboardOptions
{
    public string Directory { get; init; } = "Dashboard";
    public string Title { get; set; } = "Dashboard";



    public bool EnableAntiforgery { get; init; } = true;




    public string ThemeColor { get; set; } = "#ff6100";
    public string FavIcon { get; set; } = "favicon.svg";
}
