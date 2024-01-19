using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runner.Business.DataAccess;
using Runner.Business.Helpers;
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
                .AddSingleton(serviceProvider => new Database(configuration.GetConnectionString("Main")));

            var allServices = typeof(DataServiceBase)
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
