using Microsoft.Extensions.DependencyInjection;
using Runner.Application.Services;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Application.DependecyInjection
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddScoped<IdentityProvider>();

            var commandResultHandlerType = typeof(ICommandResultHandler<,>);
            var commandResultHandlers = GetAllTypesImplementingOpenGenericType(commandResultHandlerType, typeof(ApplicationDI).Assembly);
            foreach (var commandResultHandler in commandResultHandlers)
            {
                services
                    .AddScoped(commandResultHandler);
            }
            HandlerRegister.CommandResultHandlers.AddRange(commandResultHandlers);

            return services;
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                       (y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                       (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }
}
