using CSharpWeb.Kernel.Components;
using CSharpWeb.Kernel.Middleware;
using CSharpWeb.Kernel.Nodes;
using CSharpWeb.Kernel.Session;

namespace CSharpWeb.Kernel.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddApplication<T>(this IServiceCollection services) where T : ComponentBase
    {
        services
            .AddScoped<NodeRefService>()
            .AddScoped<SessionProvider>()
            .AddSingleton<SessionService>();

        var type = typeof(ComponentBase);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

        foreach (var comp in types)
        {
            if (comp == typeof(T))
            {
                services
                    .AddKeyedScoped<ComponentBase, T>("app");
            }
            else
            {
                services
                    .AddScoped(comp);
            }
        }

        return services;
    }

    public static void UseApplication(this WebApplication app)
    {
        app.UseWebSockets();
        app.UseMiddleware<ApplicationMiddleware>();
    }
}
