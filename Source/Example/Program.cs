using Example.Components;
using Example.Services;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Zonit.Extensions.Identity;
using Zonit.Extensions.Organizations;
using Zonit.Extensions.Projects;
using Zonit.Extensions.Website;
using Zonit.Services;
using Zonit.Services.Dashboard.DependencyInjection;
using Zonit.Services.Dashboard.Options;

namespace Example;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // ADD THIS LINE
        builder.Services.AddDashboardService();

        // PROVIDERS
        builder.Services.AddTransient<IOrganizationManager, OrganizationService>();
        builder.Services.AddTransient<IUserOrganizationManager, UserOrganization>();
        builder.Services.AddTransient<IUserProvider, UserService>();
        builder.Services.AddTransient<ISessionProvider, SessionService>();
        builder.Services.AddTransient<IOrganizationProjectManager, ProjectService>();

        builder.Services.AddExampleManager();
        builder.Services.AddExampleManagement();

        builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddConsole();
        });

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
//#if NET8_0
        app.UseStaticFiles();
//#endif
        app.UseAntiforgery();
//#if NET9_0_OR_GREATER
//        app.MapStaticAssets();
//#endif
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.UseDashboardServices<IAreaManager>(new DashboardOptions
        {
            Directory = "Manager",
            Title = "Manager",
        });
        app.UseDashboardServices<IAreaManagement>(new DashboardOptions
        {
            Directory = "management",
            Title = "Management",
        });
        app.UseDashboardServices<IAreaDiagnostic>(new DashboardOptions
        {
            Directory = "diagnostic",
            Title = "Diagnostic",
        });

        app.Run();
    }
}