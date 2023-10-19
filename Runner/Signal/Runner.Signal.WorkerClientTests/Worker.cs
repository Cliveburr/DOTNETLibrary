using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace Runner.Signal.WorkerClientTests
{
    public class KeepAlwaysConnected : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(1);
        }
    }

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HubConnection _connection;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new KeepAlwaysConnected())
                .WithUrl("http://localhost:5021/hubs/clienttoserver")
                .Build();

            _connection.On("Call", () =>
            {
                _logger.LogInformation("Received from server");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _connection.StartAsync(stoppingToken);

                    break;
                }
                catch
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }

            var result = await _connection.InvokeAsync<string>("Ping", stoppingToken);
            if (result != "PONG")
            {
                throw new Exception(result);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            await _connection.DisposeAsync();
        }
    }
}