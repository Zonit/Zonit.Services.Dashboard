using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Zonit.Dashboard.Themes;

namespace Zonit.Extensions;

/// <summary>
/// Extension surface for registering custom dashboard themes alongside the three
/// built-ins (Default / Ocean / Forest) that <c>AddDashboard()</c> seeds.
/// </summary>
public static class ServiceCollectionThemeExtensions
{
    /// <summary>
    /// Adds a custom <see cref="IDashboardTheme"/> implementation to the registry.
    /// Custom themes appear in the dashboard's theme selector alongside built-ins,
    /// in registration order — call this <em>after</em> <c>AddDashboard()</c> so
    /// the built-in defaults stay at the top of the picker.
    /// </summary>
    /// <typeparam name="TTheme">The theme implementation. Scoped lifetime so theme
    /// instances can capture per-circuit services like <c>ITenantProvider</c>
    /// without falling into singleton captive-dependency traps.</typeparam>
    public static IServiceCollection AddDashboardTheme<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TTheme>(this IServiceCollection services)
        where TTheme : class, IDashboardTheme
    {
        services.AddScoped<IDashboardTheme, TTheme>();
        return services;
    }
}
