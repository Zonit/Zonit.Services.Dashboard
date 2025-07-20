using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Translations;
using Zonit.Extensions;
using Zonit.Services.Dashboard;
using Zonit.Services.Dashboard.Data;
using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.Dashboard.Services;
using Zonit.Services.Dashboard.Repositories;
using Zonit.Extensions.Website;

namespace Zonit.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        //services.AddEventMessageService();

        services.AddCulturesExtension();
        services.AddIdentityExtension();
        services.AddCookiesExtension();
        services.AddOrganizationsExtension();
        services.AddProjectsExtension();
        services.AddNavigationsExtension();
        services.AddToastsExtension();

        services.AddBreadcrumbsExtension();

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
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 300;
            config.SnackbarConfiguration.ShowTransitionDuration = 400;
            //config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        }).AddMudTranslations();

        services.AddScoped<ISettingsManager, SettingsRepository>();
        services.AddScoped<ISettingsProvider, SettingsService>();
        services.AddTransient<IToastProvider, ToastService>();

        services.AddHostedService<CultureData>();

        services.AddDashboardApplication();

        return services;
    }
}