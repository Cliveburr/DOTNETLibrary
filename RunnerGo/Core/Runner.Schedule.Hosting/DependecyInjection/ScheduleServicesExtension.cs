using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Schedule.Hosting.Services;

namespace Runner.Schedule.Hosting.DependecyInjection
{
    public static class ScheduleServicesExtension
    {
        public static IServiceCollection AddScheduleHosting(this IServiceCollection services)
        {
            services
                .AddSingleton<ScheduleManagerService>();

            return services;
        }

        public static void InitializeScheduleManager(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var scheduleManagerService = scope.ServiceProvider.GetRequiredService<ScheduleManagerService>();
                _ = scheduleManagerService.Initialize();
            }
        }
    }
}
