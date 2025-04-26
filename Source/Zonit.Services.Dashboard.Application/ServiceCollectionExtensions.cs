using Microsoft.Extensions.DependencyInjection;
using Zonit.Services.Dashboard.Application.Services;

namespace Zonit.Services.Dashboard;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardApplication(this IServiceCollection services)
    {
        services.AddScoped<IExtensionManager, ExtensionManagerService>();

        return services;
    }
}