using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Business.Entities.Job;
using Runner.Business.Jobs;
using Runner.Script.Hosting.Jobs;

namespace Runner.Script.Hosting.DependecyInjection
{
    public static class ScriptServicesExtension
    {
        public static IServiceCollection AddScriptHosting(this IServiceCollection services)
        {
            services
                .AddScoped<ExtractScriptPackageJobHandler>();

            return services;
        }

        public static void InitializeScriptManager(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var jobMediator = scope.ServiceProvider.GetRequiredService<JobMediator>();
                jobMediator
                    .AddJobHandler<ExtractScriptPackageJobHandler>(JobType.ExtractScriptPackage);
            }
        }
    }
}
