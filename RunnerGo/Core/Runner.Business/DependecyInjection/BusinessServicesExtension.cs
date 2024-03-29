﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Helpers;
using Runner.Business.Jobs;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;

namespace Runner.Business.DependecyInjection
{
    public static class BusinessServicesExtension
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IdentityProvider>()
                .AddScoped<AuthenticationService>()
                .AddSingleton<IAgentWatcherNotification, ManualAgentWatcherNotification>()
                .AddSingleton(serviceProvider => new Database(configuration.GetConnectionString("Main")))
                .AddSingleton<JobMediator>()
                .AddScoped<CreateRunJobHandler>();

            var allServices = typeof(DataServiceBase)
                .GetAllAssignableFrom();
            foreach (var service in allServices)
            {
                services
                    .AddScoped(service);
            }

            return services;
        }

        public static void InitializeBusiness(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<Database>();
                var configurations = new CollectionConfigurations(database);
                configurations.Configure();
            }
        }

        public static void FlagToCheckJobsWaiting(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var jobMediator = scope.ServiceProvider.GetRequiredService<JobMediator>();
                jobMediator
                    .AddJobHandler<CreateRunJobHandler>(JobType.CreateRun);

                jobMediator.FlagToCheckJobsWaiting();

            }
        }
    }
}
