using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Zonit.Dashboard.Extensions;

namespace Zonit.Extensions;

/// <summary>
/// Extension surface for registering dashboard drawer / toolbar extensions.
/// Pair with <see cref="ServiceCollectionDashboardExtensions.AddDashboard"/> —
/// the registry / drawer-state singletons are wired there; these helpers only
/// register individual extension types.
/// </summary>
public static class ServiceCollectionExtensionExtensions
{
    /// <summary>
    /// Registers a drawer extension as a singleton. Use the factory overload when
    /// the extension needs DI dependencies that aren't already singletons.
    /// </summary>
    public static IServiceCollection AddDrawerExtension<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TExtension>(this IServiceCollection services)
        where TExtension : class, IDrawerExtension
    {
        services.AddSingleton<IDrawerExtension, TExtension>();
        return services;
    }

    /// <summary>
    /// Factory overload — useful when the extension type has a constructor that
    /// needs explicit wiring (constants captured in a closure, decorators wrapping
    /// other DI services). Resolved via <see cref="IServiceProvider"/>.
    /// </summary>
    public static IServiceCollection AddDrawerExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, IDrawerExtension
    {
        services.AddSingleton<IDrawerExtension>(factory);
        return services;
    }

    /// <summary>
    /// Registers a toolbar extension as a singleton.
    /// </summary>
    public static IServiceCollection AddToolbarExtension<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TExtension>(this IServiceCollection services)
        where TExtension : class, IToolbarExtension
    {
        services.AddSingleton<IToolbarExtension, TExtension>();
        return services;
    }

    /// <summary>Factory overload — see <see cref="AddDrawerExtension{TExtension}(IServiceCollection, Func{IServiceProvider, TExtension})"/>.</summary>
    public static IServiceCollection AddToolbarExtension<TExtension>(
        this IServiceCollection services,
        Func<IServiceProvider, TExtension> factory)
        where TExtension : class, IToolbarExtension
    {
        services.AddSingleton<IToolbarExtension>(factory);
        return services;
    }
}
