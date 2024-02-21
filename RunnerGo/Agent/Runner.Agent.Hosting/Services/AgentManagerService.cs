using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Model;
using Runner.Agent.Interface.Model;
using Runner.Agent.Interface.Model.Data;
using Runner.Business.Actions;
using Runner.Business.Datas.Model;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Security;
using Runner.Business.Jobs;
using Runner.Business.Security;
using Runner.Business.Services;
using Runner.Business.Services.NodeTypes;
using Runner.Business.WatcherNotification;
using System.Diagnostics.Tracing;
using System.Security.Authentication;

namespace Runner.Agent.Hosting.Services
{
    public class AgentManagerService //: IDisposable
    {
        private readonly IHubContext<AgentHub> _agentHub;
        private readonly ILogger<AgentManagerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AgentConnect> _agents;
        //private readonly IAgentWatcherNotification _agentWatcherNotification;
        private readonly JobMediator _jobMediator;

        public AgentManagerService(IHubContext<AgentHub> agentHub, ILogger<AgentManagerService> logger, IServiceProvider serviceProvider /*, IAgentWatcherNotification agentWatcherNotification*/, JobMediator jobMediator)
        {
            _agentHub = agentHub;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _agents = new List<AgentConnect>();
            //_agentWatcherNotification = agentWatcherNotification;
            _jobMediator = jobMediator;

            //_agentWatcherNotification.OnJobQueued += OnJobQueued;
            //_agentWatcherNotification.OnJobStop += OnJobStop;
        }

        //public void Dispose()
        //{
        //    _agentWatcherNotification.OnJobQueued -= OnJobQueued;
        //    _agentWatcherNotification.OnJobStop -= OnJobStop;
        //}

        public void FlagToCheckJobsWaiting()
        {
            _jobMediator.FlagToCheckJobsWaiting();
        }

        internal async Task Register(AgentHub hub, RegisterRequest request)
        {
            var scope = _serviceProvider.CreateScope();
            AgentConnect? agentConnect = null;

            try
            {
                var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
                if (!(await authenticationService.LoginByAccessToken(request.AccessToken, AccessTokenType.WebUI))) //TODO, trocar para tipo Agent
                {
                    throw new AuthenticationException();
                }

                var agentService = scope.ServiceProvider.GetRequiredService<AgentService>();
                var agentId = await agentService.Register(request.AgentPoolPath, request.MachineName, request.VersionName, request.Tags);

                var found = FindByAgent(agentId);
                if (found != null)
                {
                    try
                    {
                        found.Scope.Dispose();
                    }
                    catch { }
                    try
                    {
                        found.Hub.Dispose();
                    }
                    catch { }
                    lock (_agents)
                    {
                        _agents.Remove(found);
                    }
                }

                lock (_agents)
                {
                    agentConnect = new AgentConnect
                    {
                        ConnectionId = hub.Context.ConnectionId,
                        Hub = hub,
                        AgentPoolPath = request.AgentPoolPath,
                        AgentId = agentId,
                        Scope = scope
                    };
                    _agents.Add(agentConnect);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                try
                {
                    scope.Dispose();
                }
                catch { }
                throw;
            }

            FlagToCheckJobsWaiting();
        }

        private AgentConnect? FindByAgent(ObjectId agentId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.AgentId == agentId);
            }
        }

        private AgentConnect? FindByConnectionId(string connectionId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.ConnectionId == connectionId);
            }
        }

        public AgentConnect? FindAgent(Func<AgentConnect, bool> predicate)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(predicate);
            }
        }


        internal Task Heartbeat()
        {
            return Task.CompletedTask;
        }

        internal async Task Offline(string connectionId)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect != null)
            {
                if (agentConnect.JobRunning is not null)
                {
                    await ScriptError(connectionId, new ScriptErrorRequest
                    {
                        Message = "Agent offline!",
                        FullError = ""
                    });
                }

                var agentService = agentConnect.Scope.ServiceProvider.GetRequiredService<AgentService>();
                await agentService.UpdateOffline(agentConnect.AgentId);
                
                Remove(agentConnect);
            }
        }

        private void Remove(AgentConnect agentConnect)
        { 
            try
            {
                agentConnect.Scope?.Dispose();
            }
            catch { }

            lock (_agents)
            {
                _agents.Remove(agentConnect);
            }
        }

        //private void OnJobQueued(Job job)
        //{
        //    _ = RunQueuedJob(job);
        //}

        //private async Task CheckJobsForAgent(AgentConnect agentConnect)
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

        //        var job = await jobService.GetWaitingAndQueueOfTypes([JobType.AgentUpdate, JobType.RunAction]);

        //        if (job is not null)
        //        {
        //            await RunQueuedJob(job);
        //        }
        //    }
        //}

        //private async Task RunQueuedJob(Job job)
        //{
        //    try
        //    {
        //        switch (job.Type)
        //        {
        //            case JobType.AgentUpdate:
        //                await RunAgentUpdateJob(job);
        //                break;
        //            case JobType.RunAction:
        //                await RunActionJob(job);
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        try
        //        {
        //            using (var scope = _serviceProvider.CreateScope())
        //            {
        //                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
        //                await jobService.SetError(job.JobId, ex);
        //            }
        //        }
        //        catch
        //        {
        //            _logger.LogError(ex, null);
        //        }
        //    }
        //}

        //private async Task RunAgentUpdateJob(Job job)
        //{
        //    AgentConnect? avaiableAgent = null;
        //    lock (_agents)
        //    {
        //        avaiableAgent = _agents
        //            .Where(a => a.JobRunning is null && a.AgentId == job.AgentId)
        //            .FirstOrDefault();
        //    }
        //    if (avaiableAgent is null)
        //    {
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
        //            await jobService.SetWaiting(job.JobId);
        //        }
        //    }
        //    else
        //    {
        //        var jobService = avaiableAgent.Scope.ServiceProvider.GetRequiredService<JobService>();
        //        var agentVersionService = avaiableAgent.Scope.ServiceProvider.GetRequiredService<AgentVersionService>();

        //        await jobService.SetRunning(job.JobId, avaiableAgent.AgentId);

        //        var latestVersion = await agentVersionService.ReadLatest();
        //        if (latestVersion is null || latestVersion.FileContent is null)
        //        {
        //            throw new RunnerException("Invalid latest AgentVersion to update!");
        //        }

        //        var request = new UpdateVersionRequest
        //        {
        //            Version = latestVersion.Version,
        //            Content = latestVersion.FileContent
        //        };

        //        await _agentHub.Clients.Client(avaiableAgent.ConnectionId).SendAsync("UpdateVersion", request);

        //        await jobService.SetCompleted(job.JobId);
        //    }
        //}

        //private async Task RunActionJob(Job job)
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
        //        var runService = scope.ServiceProvider.GetRequiredService<RunService>();
        //        var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
        //        authenticationService.LoginForInternalServices();

        //        try
        //        {
        //            Assert.MustNotNull(job.RunId, "RunActionJob missing RunId, JobId: " + job.JobId);
        //            Assert.MustNotNull(job.ActionId, "RunActionJob missing ActionId, JobId: " + job.JobId);
        //        }
        //        catch (Exception ex)
        //        {
        //            await jobService.SetError(job.JobId, ex);
        //            return;
        //        }

        //        AgentConnect? agentConnected = null;

        //        try
        //        {
        //            var run = await runService.ReadById(job.RunId.Value);
        //            Assert.MustNotNull(run, "RunActionJob invalid RunId, JobId: " + job.JobId);

        //            var dataExpandService = scope.ServiceProvider.GetRequiredService<DataExpandService>();
        //            var control = ActionControl.From(run);
        //            var contextData = control.ComputeActionContextData(job.ActionId.Value, dataExpandService);

        //            var scriptPath = contextData.ReadScriptVersion("Script");
        //            Assert.MustNotNull(scriptPath, $"Run with invalid script path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value} }}");
        //            var agentPoolModeId = contextData.ReadNodePath("AgentPool");
        //            Assert.MustNotNull(agentPoolModeId, $"Run with invalid agent pool path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value} }}");

        //            var scriptService = scope.ServiceProvider.GetRequiredService<ScriptService>();
        //            var sts = await scriptService.ReadVersionByScriptPath(scriptPath.Value.ScriptNodeId, scriptPath.Value.Version);
        //            Assert.MustNotNull(sts, $"Run with invalid script path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value}, ScriptPath: {scriptPath} }}");

        //            if (sts.Value.ScriptVersion.Input?.Any() ?? false)
        //            {
        //                contextData.Merge(sts.Value.ScriptVersion.Input);
        //                await contextData.Resolve();
        //                var validation = contextData.Validate();
        //                if (validation.Any())
        //                {
        //                    var fullMsg = string.Join(Environment.NewLine, validation.Select(v => v.Text));
        //                    throw new RunnerException(fullMsg);
        //                }
        //            }
        //            else
        //            {
        //                await contextData.Resolve();
        //            }

        //            var agentService = scope.ServiceProvider.GetRequiredService<AgentService>();
        //            var agents = await agentService.ReadAgentsByAgentPoolNodeId(agentPoolModeId.Value);
        //            Assert.MustNotNull(agents, $"Run with invalid agent pool path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value}, AgentPoolPath: {agentPoolModeId} }}");

        //            var actionTags = contextData.ReadStringList("Tags")
        //                ?? new List<string>();

        //            if (agents.Any())
        //            {
        //                foreach (var agent in agents)
        //                {
        //                    var agentTags = agent.RegistredTags;
        //                    if (agent.ExtraTags is not null)
        //                    {
        //                        agentTags.AddRange(agent.ExtraTags);
        //                    }

        //                    lock (_agents)
        //                    {
        //                        agentConnected = _agents
        //                            .FirstOrDefault(a =>
        //                            {
        //                                if (a.JobRunning is not null)
        //                                {
        //                                    return false;
        //                                }

        //                                if (a.AgentId != agent.AgentId)
        //                                {
        //                                    return false;
        //                                }

        //                                return agentTags.Intersect(actionTags).Count() == agentTags.Count;
        //                            });

        //                        if (agentConnected is null)
        //                        {
        //                            continue;
        //                        }

        //                        agentConnected.JobRunning = new JobRunning
        //                        {
        //                            JobId = job.JobId,
        //                            RunId = job.RunId.Value,
        //                            ActionId = job.ActionId.Value
        //                        };
        //                    }

        //                    try
        //                    {
        //                        var runAgentService = agentConnected.Scope.ServiceProvider.GetRequiredService<RunService>();
        //                        await runAgentService.SetRunning(job.RunId.Value, job.ActionId.Value);

        //                        var jobAgentService = agentConnected.Scope.ServiceProvider.GetRequiredService<JobService>();
        //                        await jobAgentService.SetRunning(job.JobId, agent.AgentId);

        //                        var request = new RunScriptRequest
        //                        {
        //                            ScriptContentId = sts.Value.ScriptVersion.ScriptContentId.ToString(),
        //                            Assembly = sts.Value.ScriptVersion.Assembly,
        //                            FullTypeName = sts.Value.ScriptVersion.FullTypeName,
        //                            InputData = MapDataPropertyToAgent(contextData.ToDataProperty())
        //                        };

        //                        _ = _agentHub.Clients.Client(agentConnected.ConnectionId).SendAsync("RunScript", request);
        //                    }
        //                    catch
        //                    {
        //                        agentConnected.JobRunning = null;
        //                        throw;
        //                    }

        //                    return;
        //                }
        //            }

        //            await jobService.SetWaiting(job.JobId);

        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                await runService.SetError(job.RunId.Value, job.ActionId.Value, ex.Message, ex.ToString());

        //                await jobService.SetError(job.JobId, ex);
        //            }
        //            catch (Exception ex2)
        //            {
        //                await jobService.SetError(job.JobId, new Exception(ex.ToString() + Environment.NewLine + ex2.ToString()));
        //            }
        //            finally
        //            {
        //                if (agentConnected is not null)
        //                {
        //                    _ = CheckJobsForAgent(agentConnected);
        //                }
        //            }
        //        }
        //    }
        //}

        //private List<AgentDataProperty>? MapDataPropertyToAgent(List<DataProperty>? properties)
        //{
        //    return properties?
        //        .Select(p => new AgentDataProperty
        //        {
        //            Name = p.Name,
        //            Type = (AgentDataTypeEnum)p.Type,
        //            Value = p.Value is null ?
        //                null :
        //                new AgentDataValue
        //                {
        //                    StringValue = p.Value.StringValue,
        //                    IntValue = p.Value.IntValue,
        //                    StringListValue = p.Value.StringListValue,
        //                    NodePath = p.Value.NodePath,
        //                    DataExpand = MapDataPropertyToAgent(p.Value.DataExpand?.Select(d => d.ToDataProperty()).ToList())
        //                }
        //        })
        //        .ToList();
        //}

        public Task CallRunScript(string connectionId, RunScriptRequest request)
        {
            return _agentHub.Clients.Client(connectionId).SendAsync("RunScript", request);
        }

        public Task CallUpdateVersion(string connectionId, UpdateVersionRequest request)
        {
            return _agentHub.Clients.Client(connectionId).SendAsync("UpdateVersion", request);
        }

        public Task CallStopScript(string connectionId)
        {
            return _agentHub.Clients.Client(connectionId).SendAsync("StopScript");
        }

        internal async Task ScriptError(string connectionId, ScriptErrorRequest request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect is null || agentConnect.JobRunning is null)
            {
                FlagToCheckJobsWaiting();
                return;
            }

            var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
            var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();

            try
            {
                await runService.SetError(agentConnect.JobRunning.RunId, agentConnect.JobRunning.ActionId, request.Message, request.FullError);

                await jobService.SetError(agentConnect.JobRunning.JobId, request.FullError);
            }
            catch (Exception ex)
            {
                await jobService.SetError(agentConnect.JobRunning.JobId, ex);
            }
            finally
            {
                agentConnect.JobRunning = null;
                FlagToCheckJobsWaiting();
            }
        }

        internal async Task ScriptFinish(string connectionId, RunScriptResponse request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect is null || agentConnect.JobRunning is null)
            {
                FlagToCheckJobsWaiting();
                return;
            }

            var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
            var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();

            try
            {
                await runService.SetCompleted(agentConnect.JobRunning.RunId, agentConnect.JobRunning.ActionId, MapAgentToDataProperty(request.OutputData));
                
                await jobService.SetCompleted(agentConnect.JobRunning.JobId);
            }
            catch (Exception ex)
            {
                await jobService.SetError(agentConnect.JobRunning!.JobId, ex);
            }
            finally
            {
                agentConnect.JobRunning = null;
                FlagToCheckJobsWaiting();
            }
        }

        private List<DataProperty>? MapAgentToDataProperty(List<AgentDataProperty>? properties)
        {
            return properties?
                .Select(p => new DataProperty
                {
                    Name = p.Name,
                    Type = (DataTypeEnum)p.Type,
                    Value = p.Value is null ?
                        null :
                        new DataValue
                        {
                            StringValue = p.Value.StringValue,
                            IntValue = p.Value.IntValue,
                            StringListValue = p.Value.StringListValue,
                            NodePath = p.Value.NodePath,
                            DataExpand = MapAgentToDataProperty(p.Value.DataExpand)?.Select(d => new DataHandlerItem(d)).ToList()
                        }
                })
                .ToList();
        }

        internal async Task ScriptLog(string connectionId, ScriptLogRequest request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect is null || agentConnect.JobRunning is null)
            {
                FlagToCheckJobsWaiting();
                return;
            }

            var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();

            await runService.WriteLog(agentConnect.JobRunning.RunId, request.Text);
        }

        internal async Task<GetScriptResponse> GetScript(string connectionId, GetScriptRequest request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            Assert.MustNotNull(agentConnect, "AgentConnect not found! connectionId: " + connectionId);

            var scriptContentService = agentConnect.Scope.ServiceProvider.GetRequiredService<ScriptContentService>();
            var scriptcontent = await scriptContentService.ReadByIdStr(request.ScriptContentId);
            Assert.MustNotNull(scriptcontent, "ScriptContent not found! " + request.ScriptContentId);

            return new GetScriptResponse
            {
                Content = scriptcontent.FileContent
            };
        }
    }
}
