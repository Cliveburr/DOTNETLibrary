using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runner.Business.DataAccess;
using Runner.Infrastructure.DataAccess;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Read;
using Runner.Kernel.Events.Write;
using Runner.Kernel.Helpers;

namespace Runner.Infrastructure.DependecyInjection
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton(serviceProvider => new MainDatabase(configuration.GetConnectionString("Main")));

            var readHandlerType = typeof(IReadHandler<,>);
            var readHandlers = readHandlerType
                .GetAllTypesImplementingOpenGenericType(typeof(InfrastructureDI).Assembly);
            foreach (var readHandler in readHandlers)
            {
                services
                    .AddTransient(readHandler);
            }
            HandlerRegister.ReadHandlers.AddRange(readHandlers);

            var writeHandlerType = typeof(IWriteHandler<>);
            var writeHandlers = writeHandlerType
                .GetAllTypesImplementingOpenGenericType(typeof(InfrastructureDI).Assembly);
            foreach (var writeHandler in writeHandlers)
            {
                services
                    .AddTransient(writeHandler);
            }
            HandlerRegister.WriteHandlers.AddRange(writeHandlers);

            var collectionBaseType = typeof(CollectionBase<>);
            var collections = collectionBaseType
                .GetAllFromAbstract(typeof(InfrastructureDI).Assembly);
            foreach (var collection in collections)
            {
                services
                    .AddScoped(collection);
            }

            return services;
        }
    }
}
