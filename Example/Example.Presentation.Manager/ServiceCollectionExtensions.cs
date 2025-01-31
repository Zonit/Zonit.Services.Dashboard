using Example.Presentation.Manager.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Zonit.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExampleManager(this IServiceCollection services)
    {
        services.AddHostedService<NavData>();
        services.AddHostedService<CultureData>();
        return services;
    }
}