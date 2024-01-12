using Runner.WebUI.Helpers;
using Runner.WebUI.Pages.Main;
using Runner.WebUI.Services.Authentication;
using Runner.Kernel.DependecyInjection;
using Runner.Application.DependecyInjection;
using Runner.Infrastructure.DependecyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddScoped<AuthenticationService>()
    .AddScoped<GlobalJavascript>();

builder.Services
    .AddKernelServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);








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

app.Run();
