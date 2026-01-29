using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using MudBlazor.Translations;
using Zonit.Extensions;
using Zonit.Extensions.Website;
using Zonit.Services.Dashboard;
using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.Dashboard.Application;
using Zonit.Services.Dashboard.Data;
using Zonit.Services.Dashboard.DrawerExtensions;
using Zonit.Services.Dashboard.Repositories;
using Zonit.Services.Dashboard.Services;

namespace Zonit.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

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

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 300;
            config.SnackbarConfiguration.ShowTransitionDuration = 400;
        }).AddMudTranslations();

        services.AddScoped<ISettingsManager, SettingsRepository>();
        services.AddScoped<ISettingsProvider, SettingsService>();
        services.AddTransient<IToastProvider, ToastService>();

        services.AddHostedService<CultureData>();

        services.AddDashboardApplication();

        // Register built-in drawer extensions (AOT-safe with factory delegates)
        services.AddDrawerExtension<CultureDrawerExtension>(_ => new CultureDrawerExtension());
        services.AddDrawerExtension<TaskDrawerExtension>(_ => new TaskDrawerExtension());

        return services;
    }
}