using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using Runner.WebUI.Helpers;
using Runner.WebUI.Security;

namespace Runner.WebUI.DependecyInjection
{
    public static class WebUIServicesExtension
    {
        public static IServiceCollection AddWebUIServices(this IServiceCollection services)
        {
            services
                .AddScoped<WebAuthenticationService>()
                .AddScoped<GlobalJavascript>()
                .AddScoped<ModalService>()
                .AddScoped<NotificationService>();
                

            return services;
        }
    }
}
