using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Runner.Agent.Model;

namespace Runner.Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private HubConnection _connection;
        private CancellationToken _cancellationToken;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new KeepAlwaysConnected())
                .WithUrl("http://localhost:5021/hub/agent")
                .Build();

            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            _connection.Reconnecting += _connection_Reconnecting;

            _connection.On("RunScript", new Type[] { typeof(RunScriptRequest) }, RunScript);
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
                var accessToken = _configuration.GetValue<string>("AccessToken");
                var agentPool = _configuration.GetValue<string>("AgentPool");
                var tags = _configuration.GetSection("Tags").Get<List<string>>();
                if (accessToken is null || agentPool is null)
                {
                    throw new ArgumentNullException("Invalid Agent configuration!");
                }

                await _connection.InvokeAsync("Register", new RegisterRequest
                {
                    MachineName = Environment.MachineName,
                    AccessToken = accessToken,
                    AgentPool = agentPool,
                    Tags = tags ?? new List<string>()
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

        private async Task RunScript(object[] parameters)
        {
            var request = parameters.Length > 0 ?
                parameters[0] as RunScriptRequest :
                null;
            _logger.LogInformation("Run script at: {time}", DateTimeOffset.Now);

            if (request == null)
            {
                throw new ArgumentNullException("RunScriptRequest");
            }

            await ScriptStarted();

            await ScriptLog(request);

            await ScriptFinish(request);
        }

        private async Task ScriptStarted()
        {
            _logger.LogInformation("Script started at: {time}", DateTimeOffset.Now);

            try
            {
                await _connection.InvokeAsync("ScriptStarted", _cancellationToken);
            }
            catch (Exception err)
            {
                await ScriptError(err);
            }
        }

        private async Task ScriptFinish(RunScriptRequest runRequest)
        {
            _logger.LogInformation("Script finish at: {time}", DateTimeOffset.Now);

            var request = new ScriptFinishRequest
            {
            };

            try
            {
                await _connection.InvokeAsync("ScriptFinish", request, _cancellationToken);
            }
            catch (Exception err)
            {
                await ScriptError(err);
            }
        }

        private async Task ScriptLog(RunScriptRequest runRequest)
        {
            _logger.LogInformation("Script log at: {time}", DateTimeOffset.Now);

            var request = new ScriptLogRequest
            {
                Text = "test"
            };

            try
            {
                await _connection.InvokeAsync("ScriptLog", request, _cancellationToken);
            }
            catch (Exception err)
            {
                await ScriptError(err);
            }
        }

        private async Task ScriptError(Exception scriptErr)
        {
            _logger.LogInformation("Script error at: {time}", DateTimeOffset.Now);

            var request = new ScriptErrorRequest
            {
                Error = scriptErr.ToString()
            };

            try
            {
                await _connection.InvokeAsync("ScriptError", request, _cancellationToken);
            }
            catch (Exception err)
            {
                _logger.LogError(err, "ScriptError error!");
            }
        }
    }
}