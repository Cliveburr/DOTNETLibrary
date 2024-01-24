using Runner.Agent;
using Runner.Agent.Isolation;
using Runner.Script.Interface.Scripts;
using System.Runtime.Loader;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DynamicWorker>();
    })
    .Build();

host.Run();

//Console.WriteLine("test");

//var loader = new DynamicLoader(args);
//loader.Run();
