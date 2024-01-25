using Runner.Agent;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DynamicWorker>();
    })
    .Build();

host.Run();
