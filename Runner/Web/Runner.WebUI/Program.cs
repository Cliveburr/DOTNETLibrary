using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Runner.WebUI.Authentication;
using Runner.WebUI.Helpers;
using Runner.Business.Helpers;
using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using Runner.WebUI.Components;
using Runner.Agent.Hosting.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddScoped<AuthenticationService>()
    .AddScoped<GlobalJavascript>()
    .AddScoped<ModalService>()
    .AddScoped<NotificationService>()
    .AddScoped<BaseService>();

builder.Services
    .AddRunnerServices("mongodb://localhost:27017", "Runner");

builder.Services
    .AddAgentHosting();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapAgentHub();

app.Run();
