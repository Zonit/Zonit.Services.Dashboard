using System.Reflection;
using Zonit.Extensions;
using Zonit.Extensions.Website;
using Zonit.Services.Dashboard.Areas.Dashboard;
using Zonit.Services.Dashboard.Options;
using Zonit.Services.Dashboard.Repositories;
using Zonit.Services.Dashboard.Services;

namespace Zonit.Services.Dashboard.DependencyInjection;

public static class AppMiddlewareExtensions
{
    public static IApplicationBuilder UseDashboardServices<T>(this WebApplication builder, DashboardOptions options) where T : IArea
    {
        builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/" + options.Directory, StringComparison.OrdinalIgnoreCase), app =>
        {
            app.Use(async (context, next) =>
            {
                var settings = context.RequestServices.GetRequiredService<ISettingsManager>();
                settings.SetSettings(options);
                settings.SetArea(typeof(T));

                await next();
            });


            app.UsePathBase("/" + options.Directory + "/");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;
                var request = context.HttpContext.Request;
                var pathBase = request.PathBase;

                switch (response.StatusCode)
                {
                    case 401:
                        response.Redirect($"{pathBase}/Errors/401");
                        break;
                    case 403:
                        response.Redirect($"{pathBase}/Errors/403");
                        break;
                    case 404:
                        response.Redirect($"{pathBase}/Errors/404");
                        break;
                    case 500:
                        response.Redirect($"{pathBase}/Errors/500");
                        break;
                }

                return Task.CompletedTask;
            });

            app.UseCookiesExtension();
            app.UseCulturesExtension();
            app.UseIdentityExtension();
            app.UseOrganizationsExtension(); // It has to be after UseIdentityExtension!
            app.UseProjectsExtension();

            app.UseEndpoints(endpoints =>
            {
                Type interfaceType = typeof(T);

                // Tworzenie instancji Routes<T> dynamicznie
                var routeInstance = Activator.CreateInstance(typeof(AssemblyScannerService<>).MakeGenericType(interfaceType));

                // Pobranie metod i wywołanie "Types()"
                var typesMethod = routeInstance?.GetType().GetMethod("Types");
                var assemblies = (typesMethod?.Invoke(routeInstance, null) as IEnumerable<Assembly>)?.ToArray() ?? [];

                var razor = endpoints.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode()
                    .AddAdditionalAssemblies(assemblies);

                // Dodanie wymaganej autoryzacji
                if (options.Permission is not null)
                    razor.RequireAuthorization(options.Permission);

            });
        });

        return builder;
    }
}
