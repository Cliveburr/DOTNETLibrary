using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Script.Hosting.Services;

namespace Runner.Script.Hosting.DependecyInjection
{
    public static class ScriptServicesExtension
    {
        public static IServiceCollection AddScriptHosting(this IServiceCollection services)
        {
            services
                .AddSingleton<ScriptManagerService>();

            return services;
        }

        public static void InitializeScriptManager(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var scriptManagerService = scope.ServiceProvider.GetRequiredService<ScriptManagerService>();
                _ = scriptManagerService.CheckJobsForExtractScript();
            }
        }
    }
}
