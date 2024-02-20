using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Schedule.Hosting.Services;

namespace Runner.Schedule.Hosting.DependecyInjection
{
    public static class ScheduleServicesExtension
    {
        public static IServiceCollection AddScriptHosting(this IServiceCollection services)
        {
            services
                .AddSingleton<ScheduleManagerService>();

            return services;
        }

        public static void InitializeScriptManager(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var scriptManagerService = scope.ServiceProvider.GetRequiredService<ScheduleManagerService>();
                _ = scriptManagerService.CheckScheduleTickers();
            }
        }
    }
}
