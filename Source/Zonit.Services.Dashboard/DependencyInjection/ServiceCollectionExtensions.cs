using MudBlazor;
using MudBlazor.Services;
using Zonit.Extensions;

namespace Zonit.Services.Dashboard.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddCulturesExtension();
        services.AddIdentityExtension();            // Fixed
        services.AddCookiesExtension();
        services.AddOrganizationsExtension();       // Fixed
        services.AddProjectsExtension();            // Fixed
        services.AddNavigationsExtension();

        services.AddAntiforgery();

        services
            .AddRazorComponents()
            .AddInteractiveServerComponents();
        //.AddHubOptions(options =>
        //{
        //    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        //    options.EnableDetailedErrors = false;
        //    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        //    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        //    options.MaximumParallelInvocationsPerClient = 1;
        //    options.MaximumReceiveMessageSize = 4 * 128 * 1024;
        //    options.StreamBufferCapacity = 10;
        //});

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            //config.SnackbarConfiguration.PreventDuplicates = false;
            //config.SnackbarConfiguration.NewestOnTop = false;
            //config.SnackbarConfiguration.ShowCloseIcon = true;
            //config.SnackbarConfiguration.VisibleStateDuration = 10000;
            //config.SnackbarConfiguration.HideTransitionDuration = 500;
            //config.SnackbarConfiguration.ShowTransitionDuration = 500;
            //config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddMudServices();

        return services;
    }
}