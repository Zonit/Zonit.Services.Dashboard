using Microsoft.Extensions.DependencyInjection;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Application;

/// <summary>
/// Extension methods for registering dashboard extensions in a AOT-safe manner.
/// Uses factory delegates to avoid reflection-based instantiation.
/// </summary>
public static class DashboardExtensionRegistration
{
    /// <summary>
    /// Registers a navigation extension using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The navigation extension type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNavigationExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, INavigationExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<INavigationExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }

    /// <summary>
    /// Registers a drawer extension using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The drawer extension type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDrawerExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, IDrawerExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<IDrawerExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }

    /// <summary>
    /// Registers a toolbar extension using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The toolbar extension type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddToolbarExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, IToolbarExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<IToolbarExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }

    /// <summary>
    /// Registers a settings extension using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The settings extension type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSettingsExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, ISettingsExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<ISettingsExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }

    /// <summary>
    /// Registers any dashboard extension using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDashboardExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, IDashboardExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }

    /// <summary>
    /// Registers a navigation extension and drawer extension combined using a factory.
    /// AOT-safe: Uses factory delegate instead of reflection-based instantiation.
    /// </summary>
    /// <typeparam name="TExtension">The extension type implementing both interfaces.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the extension instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNavigationWithDrawerExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, INavigationExtension, IDrawerExtension
    {
        services.AddSingleton(factory);
        services.AddSingleton<IDashboardExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<INavigationExtension>(sp => sp.GetRequiredService<TExtension>());
        services.AddSingleton<IDrawerExtension>(sp => sp.GetRequiredService<TExtension>());
        return services;
    }
}
