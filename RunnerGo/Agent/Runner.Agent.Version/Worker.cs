using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runner.Agent.Interface.Model;
using Runner.Common.Helpers;
using Runner.Agent.Version.Scripts;
using Runner.Agent.Version.Vers;
using System.Runtime.CompilerServices;

namespace Runner.Agent.Version
{
    public static class WorkerEntry
    {
        public static bool ShouldReload { get; set; } = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Execute(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            using(var scope = serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Worker>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var worker = new Worker(logger, configuration);
                try
                {
                    worker.ExecuteAsync(stoppingToken).Wait();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, null);
                }

                return ShouldReload;
            }
        }
    }

    public class Worker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private HubConnection _connection;
        private CancellationToken _stoppingToken;
        private CancellationTokenSource? _reloadSource;
        private string _versionName;
        private bool _isWorking;
        private CancellationTokenSource? _executionCancellation;

        public Worker(ILogger<Worker> looger, IConfiguration configuration)
        {
            _logger = looger;
            _configuration = configuration;

            var hubHost = configuration["HubHost"];
            if (string.IsNullOrEmpty(hubHost))
            {
                throw new ArgumentNullException(nameof(hubHost));
            }

            _versionName = VersionInfo.ReadVersionActual();

            _connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new KeepAlwaysConnected())
                .WithUrl(hubHost)
                .Build();

            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            _connection.Reconnecting += _connection_Reconnecting;

            _connection.On("RunScript", [typeof(RunScriptRequest)], RunScript);
            _connection.On("StopScript", [], StopScript);
            _connection.On("UpdateVersion", [typeof(UpdateVersionRequest)], UpdateVersion);
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Execute begin at: {time}", DateTimeOffset.Now);

            _reloadSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            _stoppingToken = _reloadSource.Token;

            await StartAsync();

            await Register();

            var heartBeat = 3000;
            int.TryParse(_configuration["HeartBeat"], out heartBeat);

            while (!_stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(heartBeat, _stoppingToken);

                if (_connection.State == HubConnectionState.Connected && !_stoppingToken.IsCancellationRequested)
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
            if (_stoppingToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Register();
            }
        }

        private async Task StartAsync()
        {
            _logger.LogInformation("Starting at: {time}", DateTimeOffset.Now);

            while (true)
            {
                try
                {
                    await _connection.StartAsync(_stoppingToken);
                    return;
                }
                catch (Exception err)
                {
                    try
                    {
                        _logger.LogError(err, "Starting error!");
                    } catch { }
                    await Task.Delay(3000, _stoppingToken);
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
                    VersionName = _versionName,
                    MachineName = Environment.MachineName,
                    AccessToken = accessToken,
                    AgentPoolPath = agentPoolPath,
                    Tags = tags ?? new List<string>()
                }, _stoppingToken);
            }
            catch (Exception err)
            {
                try
                {
                    await _connection.StopAsync(_stoppingToken);
                } catch { }
                _logger.LogError(err, "Register error!");
            }
        }

        private async Task Heartbeat()
        {
            _logger.LogInformation("Heart beating version {vers} at: {time}", _versionName, DateTimeOffset.Now);

            try
            {
                await _connection.InvokeAsync("Heartbeat", _stoppingToken);
            }
            catch (Exception err)
            {
                try
                {
                    _logger.LogError(err, "Heartbeat error!");
                } catch { }
            }
        }

        private Task UpdateVersion(object?[] parameters)
        {
            var request = parameters?.Length > 0 ?
                parameters[0] as UpdateVersionRequest :
                null;
            _logger.LogInformation("update version at: {time}", DateTimeOffset.Now);

            if (request == null)
            {
                throw new ArgumentNullException("UpdateVersionRequest");
            }

            var versionName = VersionManager.VersionName(request);
            if (_versionName == versionName)
            {
                return Task.CompletedTask;
            }

            if (_isWorking)
            {
                throw new Exception("Agent is busy!");
            }

            _isWorking = true;

            var versionPath = VersionManager.VersionDirectory(request);

            IO.ClearDirectory(versionPath);

            Zip.Descompat(request.Content, versionPath);

            _ = Task.Run(() =>
            {
                VersionInfo.PerformUpgrade(versionName);

                WorkerEntry.ShouldReload = true;

                try
                {
                    _connection.StopAsync(_stoppingToken).Wait();
                } catch { }
                if (_reloadSource is not null && !_reloadSource.IsCancellationRequested)
                {
                    _reloadSource.Cancel();
                }
            });

            return Task.CompletedTask;
        }


        private async Task RunScript(object?[] parameters)
        {
            var request = parameters?.Length > 0 ?
                parameters[0] as RunScriptRequest :
                null;
            _logger.LogInformation("Run script at: {time}", DateTimeOffset.Now);

            try
            {
                if (request is null)
                {
                    throw new ArgumentNullException("RunScriptRequest");
                }

                _ = RunScriptDetached(request);
            }
            catch (Exception ex)
            {
                try
                {
                    await _connection.InvokeAsync("ScriptError",
                        new ScriptErrorRequest
                        {
                            Error = ex.ToString()
                        }, _stoppingToken);
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "ScriptError error!");
                }
            }
        }

        private async Task RunScriptDetached(RunScriptRequest request)
        {
            try
            {
                if (request is null)
                {
                    throw new ArgumentNullException("RunScriptRequest");
                }

                if (!ScriptsManager.CheckIfExist(request))
                {
                    var getScriptResponse = await _connection.InvokeAsync<GetScriptResponse>("GetScript",
                        new GetScriptRequest
                        {
                            ScriptContentId = request.ScriptContentId
                        }, _stoppingToken);
                    if (getScriptResponse is null)
                    {
                        throw new ArgumentNullException("GetScriptRequest");
                    }
                    ScriptsManager.Create(request, getScriptResponse);
                }

                _executionCancellation = new CancellationTokenSource();
                var executeCancelationToken = CancellationTokenSource.CreateLinkedTokenSource(_executionCancellation.Token, _stoppingToken);
                var scriptIsolation = new ScriptIsolation(request, ScriptLog);
                var result = await scriptIsolation.Execute(executeCancelationToken.Token);
                _executionCancellation = null;

                await _connection.InvokeAsync("ScriptFinish",
                    new RunScriptResponse
                    {
                        IsSuccess = result.IsSuccess,
                        ErrorMessage = result.ErrorMessage,
                        Data = result.Data,
                    }, _stoppingToken);
            }
            catch (Exception ex)
            {
                try
                {
                    await _connection.InvokeAsync("ScriptError",
                        new ScriptErrorRequest
                        {
                            Error = ex.ToString()
                        }, _stoppingToken);
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "ScriptError error!");
                }
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

        private async Task ScriptLog(string text)
        {
            _logger.LogInformation("Script log at: {time}", DateTimeOffset.Now);

            var request = new ScriptLogRequest
            {
                Text = text
            };

            await _connection.InvokeAsync("ScriptLog", request, _stoppingToken);
        }
    }
}
