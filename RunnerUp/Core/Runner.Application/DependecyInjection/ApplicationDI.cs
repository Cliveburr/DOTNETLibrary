using Microsoft.Extensions.DependencyInjection;
using Runner.Application.Services;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;
using Runner.Kernel.Helpers;

namespace Runner.Application.DependecyInjection
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddScoped<IdentityProvider>();

            var commandResultHandlerType = typeof(ICommandResultHandler<,>);
            var commandResultHandlers = commandResultHandlerType
                .GetAllTypesImplementingOpenGenericType(typeof(ApplicationDI).Assembly);
            foreach (var commandResultHandler in commandResultHandlers)
            {
                services
                    .AddTransient(commandResultHandler);
            }
            HandlerRegister.CommandResultHandlers.AddRange(commandResultHandlers);

            var commandHandlerType = typeof(ICommandHandler<>);
            var commandHandlers = commandHandlerType
                .GetAllTypesImplementingOpenGenericType(typeof(ApplicationDI).Assembly);
            foreach (var commandHandler in commandHandlers)
            {
                services
                    .AddTransient(commandHandler);
            }
            HandlerRegister.CommandHandlers.AddRange(commandHandlers);

            return services;
        }
    }
}
