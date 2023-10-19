using Microsoft.AspNetCore.SignalR;
using Runner.Signal.WorkerInterfaceTests;
using Runner.Signal.WorkerServerTests.Service;

namespace Runner.Signal.WorkerTests
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<ClientToServer> _client;

        public Worker(ILogger<Worker> logger, IHubContext<ClientToServer> client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _client.Clients.All.SendAsync("Call");

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}