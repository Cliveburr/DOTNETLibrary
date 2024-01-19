using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Runner.Agent.Interface.Model;
using Runner.Agent.Isolation;
using Runner.Agent.Scripts;
using System.Collections.Generic;

namespace Runner.Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private HubConnection _connection;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource? _executionCancellation;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var hubHost = configuration["HubHost"];
            if (string.IsNullOrEmpty(hubHost) )
            {
                throw new ArgumentNullException(nameof(hubHost));
            }

            _connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new KeepAlwaysConnected())
                .WithUrl(hubHost)
                .Build();

            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            _connection.Reconnecting += _connection_Reconnecting;

            _connection.On("RunScript", [typeof(RunScriptRequest)], RunScript);
            _connection.On("StopScript", [], StopScript);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;

            await StartAsync();

            await Register();

            var heartBeat = 3000;
            int.TryParse(_configuration["HeartBeat"], out heartBeat);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(heartBeat, stoppingToken);

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
                var agentPoolPath = _configuration.GetValue<string>("AgentPoolPath");
                var tags = _configuration.GetSection("Tags").Get<List<string>>();
                if (accessToken is null || agentPoolPath is null)
                {
                    throw new ArgumentNullException("Invalid Agent configuration!");
                }

                await _connection.InvokeAsync("Register", new RegisterRequest
                {
                    MachineName = Environment.MachineName,
                    AccessToken = accessToken,
                    AgentPoolPath = agentPoolPath,
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

        private async Task RunScript(object?[] parameters)
        {
            var request = parameters?.Length > 0 ?
                parameters[0] as RunScriptRequest :
                null;
            _logger.LogInformation("Run script at: {time}", DateTimeOffset.Now);

            if (request == null)
            {
                throw new ArgumentNullException("RunScriptRequest");
            }

            await ScriptStarted();

            if (!ScriptsManager.CheckIfExist(request))
            {
                var getScriptResponse = await GetScript(new GetScriptRequest
                {
                    Id = request.Id,
                    Version = request.Version
                });
                if (getScriptResponse is null)
                {
                    return;
                }
                ScriptsManager.Create(request, getScriptResponse);
            }

            var _executionCancellation = new CancellationTokenSource();
            var executeCancelationToken = CancellationTokenSource.CreateLinkedTokenSource(_executionCancellation.Token, _cancellationToken);
            var result = await ScriptIsolation.Execute(request, ScriptLog, executeCancelationToken.Token);
            _executionCancellation = null;

            var scriptFinishRequest = new ScriptFinishRequest
            {
                IsSuccess = result.IsSuccess,
                ContinueOnError = result.ContinueOnError,
                ErrorMessage = result.ErrorMessage,
                Output = result.Output?.Changes,
            };

            await ScriptFinish(scriptFinishRequest);
        }

        private async Task<GetScriptResponse?> GetScript(GetScriptRequest request)
        {
            _logger.LogInformation("Script started at: {time}", DateTimeOffset.Now);

            try
            {
                return await _connection.InvokeAsync<GetScriptResponse>("GetScript", request, _cancellationToken);
            }
            catch (Exception err)
            {
                await ScriptError(err);
                return null;
            }
        }

        private Task StopScript(object?[] parameters)
        {
            _logger.LogInformation("Stop script at: {time}", DateTimeOffset.Now);

            if (_executionCancellation is not null && !_executionCancellation.IsCancellationRequested)
            {
                _executionCancellation.Cancel();
            }

            return Task.CompletedTask;
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

        private async Task ScriptFinish(ScriptFinishRequest request)
        {
            _logger.LogInformation("Script finish at: {time}", DateTimeOffset.Now);

            try
            {
                await _connection.InvokeAsync("ScriptFinish", request, _cancellationToken);
            }
            catch (Exception err)
            {
                await ScriptError(err);
            }
        }

        private async Task ScriptLog(string text)
        {
            _logger.LogInformation("Script log at: {time}", DateTimeOffset.Now);

            var request = new ScriptLogRequest
            {
                Text = text
            };

            await _connection.InvokeAsync("ScriptLog", request, _cancellationToken);
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