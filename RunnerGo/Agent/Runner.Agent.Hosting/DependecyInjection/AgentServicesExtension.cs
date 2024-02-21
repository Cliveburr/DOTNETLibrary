using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Jobs;
using Runner.Agent.Hosting.Services;
using Runner.Business.Entities.Job;
using Runner.Business.Jobs;

namespace Runner.Agent.Hosting.DependecyInjection
{
    public static class AgentServicesExtension
    {
        public static IServiceCollection AddAgentHosting(this IServiceCollection services)
        {
            services
                .AddSignalR();

            services
                .AddSingleton<AgentManagerService>()
                .AddScoped<AgentUpdateJobHandler>()
                .AddScoped<RunScriptJobHandler>()
                .AddScoped<StopScriptJobHandler>();

            return services;
        }

        public static void InitializeAgentHosting(this WebApplication app)
        {
            app
                .MapHub<AgentHub>("/hub/agent");

            using (var scope = app.Services.CreateScope())
            {
                _ = scope.ServiceProvider.GetRequiredService<AgentManagerService>();

                var jobMediator = scope.ServiceProvider.GetRequiredService<JobMediator>();
                jobMediator
                    .AddJobHandler<AgentUpdateJobHandler>(JobType.AgentUpdate)
                    .AddJobHandler<RunScriptJobHandler>(JobType.RunScript)
                    .AddJobHandler<StopScriptJobHandler>(JobType.StopScript);
            }
        }
    }
}
