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
                await _connection.InvokeAsync("Register", new RegisterRequest
                {
                    MachineName = Environment.MachineName,
                    AccessToken = "EoJWeoOcNUBhZzEjRPKsHOUrgrOWy1DtBj/k7HGT5VI0NnJKloBlokBo0qGB+bjurMZWxspAV5/E2pvm9t2Xka0T32sIbGqDu61SrhogsDUBlas0Bp7w1KPRkRM1XgvA+6S9WCjxAtoE2akIU0kHgFkuuDznZDL59gM8qDikmP71upEebGpGMAmuacXPbIm74JWtZQxAqlJM6SaEVEZnqNCSSoezmAXuXljlBfAoav/ckEfiJ7i5FN7WTSxYZTwnOt2Jkkw2AravPXCJ/CDSqjomkExtrDvjXTtmQ0BP2tSYo0R7b4xerH6CwG4lqCxcdBazyGCtjpL4XOy/ejtlMbYegZTDWp1jaXQsCii21UpbcqlPGnoZj3GVRelCoaBX2KrWA7ykYBJ38AV8ig3dx7RGCE1IbjsA49mciwPWFQzFj+r+jy+w3Xav9dVIkJogLHpgKkljrItKeJGx8RbCkFQuSIaC4cVgfox+9ghjl3+1u17xExyYR9VaFdGgdBQqiRnQw5WbRgwyGaERF1EVAMOt7qJc9aAJXRUQUHnwxwNLIbQKeEbxPz6YukSjFQDeuUjFVmZrQsBxNqtJsNpAozmK2Zpwe4zRM+xNpa7NRZzF3wZsTTPmWZkqTuQj3S2hSZ5eu308DvhwDe5ZtwPXznGMAXOcXiiWMdP2R2A6DxA=",
                    AgentPool = "/test/pasta/agents",
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