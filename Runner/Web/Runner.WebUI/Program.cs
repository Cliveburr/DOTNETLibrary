using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Runner.WebUI.Authentication;
using Runner.WebUI.Helpers;
using Runner.Business.Helpers;
using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using Runner.WebUI.Components;
using Runner.Agent.Hosting.Helpers;
using Runner.WebUI.Pages.Main;
using Runner.Agent.Hosting.Services;
using Runner.Business.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddScoped<WebAuthenticationService>()
    .AddScoped<GlobalJavascript>()
    .AddScoped<ModalService>()
    .AddScoped<NotificationService>()
    .AddScoped<BaseService>()
    .AddScoped<ClipboardInterop>();

builder.Services
    .AddRunnerServices("mongodb://localhost:27017", "Runner");

builder.Services
    .AddAgentHosting();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapAgentHub();

app.Run();
