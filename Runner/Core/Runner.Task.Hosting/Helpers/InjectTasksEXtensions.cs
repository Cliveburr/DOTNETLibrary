using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Task.Hosting.Helpers
{
    public static class InjectTasksEXtensions
    {
        public static IServiceCollection AddTasksHosting(this IServiceCollection services)
        {
            services
                .AddHostedService<TasksWorker>();

            //services
           //     .AddSingleton<AgentManagerService>();

            return services;
        }
    }
}
