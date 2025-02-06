using Zonit.Extensions.Website.Abstractions.Navigations.Types;

namespace Zonit.Services.Dashboard.Options;

public class DashboardOptions
{
    private string _directory = "dashboard";
    public string Directory
    {
        get => _directory.ToLower();
        set => _directory = value ?? string.Empty;
    }
    public string Title { get; set; } = "Dashboard";

    /// <summary>
    /// Required rights policy
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Custom snippet for the dashboard
    /// </summary>
    public string? CustomSnippet { get; set; }



    public bool EnableAntiforgery { get; init; } = true;




    public string ThemeColor { get; set; } = "#ff6100";
    public string FavIcon { get; set; } = "favicon.svg";
}
