using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Hosting.Helpers
{
    public static class InjectServicesExtension
    {
        public static IServiceCollection AddAgentHosting(this IServiceCollection services)
        {
            services
                .AddSignalR();

            services
                .AddSingleton<AgentManagerService>();

            return services;
        }

        public static void MapAgentHub(this WebApplication app)
        {
            app.MapHub<AgentHub>("/hub/agent");

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;

            //    var agentManagerService = services.GetRequiredService<AgentManagerService>();
            //    agentManagerService.CheckForAgentsMissStatus();
            //}
        }
    }
}
