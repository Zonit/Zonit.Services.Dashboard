﻿using Example.Presentation.Management.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Zonit.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExampleManagement(this IServiceCollection services)
    {
        services.AddHostedService<NavData>();
        services.AddHostedService<CultureData>();
        return services;
    }
}