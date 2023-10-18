using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Process.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Helpers
{
    public static class ServiceCallerExtension
    {
        public static IServiceCollection AddServiceCallerBack<T>(this IServiceCollection services) where T : ServiceCallerBase
        {
            return services
                .AddScoped<ServiceCallerBackObj>()
                .AddScoped<ServiceCallerBack<T>>();
        }
    }
}
