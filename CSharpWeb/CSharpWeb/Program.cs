using CSharpWeb.Kernel.Extensions;
using CSharpWeb.Main;
using System.Text.Json.Serialization;

var contentRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../wwwroot"));

var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions
{
    WebRootPath = contentRootPath
});

builder.WebHost
    .UseKestrelCore()
    .ConfigureKestrel(options =>
    {

        options.ListenAnyIP(5240);

    });

builder.Services
    .AddApplication<App>();










var app = builder.Build();

app.UseStaticFiles();

app.UseApplication();

app.Run();
