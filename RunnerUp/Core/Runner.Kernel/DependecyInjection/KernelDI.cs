using Runner.Kernel.Events.Command;
using Runner.Kernel.Events;
using Microsoft.Extensions.DependencyInjection;
using Runner.Kernel.Services;
using Runner.Kernel.Mapper;

namespace Runner.Kernel.DependecyInjection
{
    public static class KernelDI
    {
        public static IServiceCollection AddKernelServices(this IServiceCollection services)
        {
            services
                .AddSingleton<EasyMapper>()
                .AddTransient<KernelService>();

            return services;
        }
    }
}
