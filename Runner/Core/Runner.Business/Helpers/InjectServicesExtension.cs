using Microsoft.Extensions.DependencyInjection;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.WatcherNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Helpers
{
    public static class InjectServicesExtension
    {
        public static IServiceCollection AddRunnerServices(this IServiceCollection services, string connectionString, string mainDatabaseName)
        {
            services
                .AddScoped<UserLogged>()
                .AddScoped<AuthenticationService>()
                .AddSingleton<IAgentWatcherNotification, ManualAgentWatcherNotification>()
                .AddSingleton(serviceProvider => new Database(connectionString, mainDatabaseName));

            var allServices = typeof(ServiceBase)
                .GetAllAssignableFrom();
            foreach (var service in allServices)
            {
                services
                    .AddScoped(service);
            }

            return services;
        }
    }
}
