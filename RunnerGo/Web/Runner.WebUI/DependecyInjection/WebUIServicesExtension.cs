using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using Runner.WebUI.JSInterop;
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
                .AddScoped<ClipboardInterop>()
                .AddScoped<ModalService>()
                .AddScoped<NotificationService>();
                

            return services;
        }
    }
}
