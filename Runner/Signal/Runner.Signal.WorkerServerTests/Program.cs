using Runner.Signal.WorkerServerTests.Service;
using Runner.Signal.WorkerTests;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();

//host.Run();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapHub<ClientToServer>("/hubs/clienttoserver");

app.Run();
