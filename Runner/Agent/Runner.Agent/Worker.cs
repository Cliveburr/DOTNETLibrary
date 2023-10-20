using Microsoft.AspNetCore.SignalR.Client;
using Runner.Agent.Model;

namespace Runner.Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HubConnection _connection;
        private CancellationToken _cancellationToken;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new KeepAlwaysConnected())
                .WithUrl("http://localhost:5021/hub/agent")
                .Build();

            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            _connection.Reconnecting += _connection_Reconnecting;

            _connection.On("RunScript", RunScript);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;

            await StartAsync();

            await Register();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(3000, stoppingToken);

                if (_connection.State == HubConnectionState.Connected)
                {
                    await Heartbeat();
                }
            }

            await _connection.DisposeAsync();
        }

        private Task _connection_Reconnecting(Exception? arg)
        {
            _logger.LogInformation("Reconnecting at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        private Task _connection_Closed(Exception? arg)
        {
            _logger.LogInformation("Closed at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        private Task _connection_Reconnected(string? arg)
        {
            _logger.LogInformation("Reconnected at: {time}", DateTimeOffset.Now);
            return Register();
        }

        private async Task StartAsync()
        {
            _logger.LogInformation("Starting at: {time}", DateTimeOffset.Now);

            while (true)
            {
                try
                {
                    await _connection.StartAsync(_cancellationToken);
                    return;
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "Starting error!");
                    await Task.Delay(3000, _cancellationToken);
                }
            }
        }

        private async Task Register()
        {
            _logger.LogInformation("Registring at: {time}", DateTimeOffset.Now);

            try
            {
                await _connection.InvokeAsync("Register", new RegisterRequest
                {
                    MachineName = Environment.MachineName,
                    AccessToken = "Fn1wtPGlR/SaVVt3lXBg1lZkMk1cfyQGqQkuyEwvnzy5ETjk5ySpK9OGFcrXwVdq5il2IwcKYoS6WA7KpJmUfOAtMVMQO4dK+4WcxP1tGR/sXZYZ6VT4E09J9OYtm2t3sEHeW42ZYhASWwRF3B3/6iJ2as5pQu8NlAYLixKEnxlW6ejaqKxGNLA8jDWerrl4UbtEUv+TVM5AT5xOBuEEQvNSTU9nikCcqg/+TG07o1mvT1ok8GSnRuXxRg5XNnrGQwscpQQYE1BwOVeCqwRVno2QN7HC9uTQYVZEA4N4X4Ro//3FjTmI83Yb8J3AhOzUC96uKbTcr6hCLlpDNISi43BQ9sFjkEujQyNqPal0in/uXrkZO+k+onFhy2Ecqaxax6W1eyIjeD6+l1RUm8FFVeaffgEC4q4dPkbVaBv9kS9f04YSlLp9YaAT+Omie0l4woEo9+JE2JSucGkhpN/+VfXS59nqqiENua2wMX0TuREbyKLa2jcrRwuo+YI9TaimO37xZQ0DeaGkQa+2evTfQudN/M4kaH5ShQwW1CNYmTAhxrYaqArCNrobXtO6i35F2mMdxqHYMUEgIunQ//WX39O40pf+POzO1i42VwI+T8TzAlmDr7YNIzAZ2cm12z9o/5R/Gjn4uygP7rw3euKNDGpPfa+jM823qLBzA05gMro=",
                    AgentPool = "/test/agents",
                    Tags = new List<string>()
                }, _cancellationToken);
            }
            catch (Exception err)
            {
                await _connection.StopAsync(_cancellationToken);
                _logger.LogError(err, "Register error!");
            }
        }

        private async Task Heartbeat()
        {
            _logger.LogInformation("Heart beating at: {time}", DateTimeOffset.Now);

            try
            {
                await _connection.InvokeAsync("Heartbeat", _cancellationToken);
            }
            catch (Exception err)
            {
                _logger.LogError(err, "Heartbeat error!");
            }
        }

        private Task RunScript()
        {
            _logger.LogInformation("Run script at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }
    }
}