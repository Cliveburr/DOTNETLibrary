using Runner.Agent.Hosting.DependecyInjection;
using Runner.WebUI.Pages.Main;
using Runner.WebUI.DependecyInjection;
using Runner.Business.DependecyInjection;
using Runner.Script.Hosting.DependecyInjection;
using Runner.Schedule.Hosting.DependecyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddWebUIServices()
    .AddBusinessServices(builder.Configuration)
    .AddAgentHosting()
    .AddScriptHosting()
    .AddScheduleHosting();







var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.InitializeBusiness();
app.InitializeAgentHosting();
app.InitializeScriptManager();
app.InitializeScheduleManager();
app.FlagToCheckJobsWaiting();

app.Run();
