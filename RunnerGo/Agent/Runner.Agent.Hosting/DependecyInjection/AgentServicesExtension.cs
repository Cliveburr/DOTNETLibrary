﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Services;

namespace Runner.Agent.Hosting.DependecyInjection
{
    public static class AgentServicesExtension
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
        }
    }
}