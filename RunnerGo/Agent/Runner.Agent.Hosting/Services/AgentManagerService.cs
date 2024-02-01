using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Interface.Model;
using Runner.Business.Actions;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Security;
using Runner.Business.Security;
using Runner.Business.Services;
using Runner.Business.Services.NodeTypes;
using Runner.Business.WatcherNotification;
using System.Security.Authentication;

namespace Runner.Agent.Hosting.Services
{
    public class AgentManagerService : IDisposable
    {
        private readonly IHubContext<AgentHub> _agentHub;
        private readonly ILogger<AgentManagerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AgentConnect> _agents;
        private readonly IAgentWatcherNotification _agentWatcherNotification;

        private class AgentConnect
        {
            public required string ConnectionId { get; set; }
            public required AgentHub Hub { get; set; }
            public required string AgentPoolPath { get; set; }
            public ObjectId AgentId { get; set; }
            public required IServiceScope Scope { get; set; }
            public ObjectId? RunningJobId { get; set; }
        }

        public AgentManagerService(IHubContext<AgentHub> agentHub, ILogger<AgentManagerService> logger, IServiceProvider serviceProvider, IAgentWatcherNotification agentWatcherNotification)
        {
            _agentHub = agentHub;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _agents = new List<AgentConnect>();
            _agentWatcherNotification = agentWatcherNotification;

            _agentWatcherNotification.OnJobQueued += OnJobQueued;
            _agentWatcherNotification.OnJobStop += OnJobStop;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobQueued -= OnJobQueued;
            _agentWatcherNotification.OnJobStop -= OnJobStop;
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

            _ = CheckJobsForAgent(agentConnect);
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

        private AgentConnect? FindByJobId(ObjectId jobId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.RunningJobId == jobId);
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
                if (agentConnect.RunningJobId.HasValue)
                {
                    await ScriptError(connectionId, new ScriptErrorRequest
                    {
                        Error = "Agent offline!"
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

        private void OnJobQueued(Job job)
        {
            _ = RunQueuedJob(job);
        }

        private async Task CheckJobsForAgent(AgentConnect agentConnect)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

                var job = await jobService.GetWaitingAndQueueOfTypes([JobType.AgentUpdate, JobType.RunAction]);

                if (job is not null)
                {
                    await RunQueuedJob(job);
                }
            }
        }

        private async Task RunQueuedJob(Job job)
        {
            try
            {
                switch (job.Type)
                {
                    case JobType.AgentUpdate:
                        await RunAgentUpdateJob(job);
                        break;
                    case JobType.RunAction:
                        await RunActionJob(job);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                        await jobService.SetError(job, ex);
                    }
                }
                catch
                {
                    _logger.LogError(ex, null);
                }
            }
        }

        private async Task RunAgentUpdateJob(Job job)
        {
            AgentConnect? avaiableAgent = null;
            lock (_agents)
            {
                avaiableAgent = _agents
                    .Where(a => !a.RunningJobId.HasValue && a.AgentId == job.AgentId)
                    .FirstOrDefault();
            }
            if (avaiableAgent is null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                    await jobService.SetWaiting(job);
                }
            }
            else
            {
                var jobService = avaiableAgent.Scope.ServiceProvider.GetRequiredService<JobService>();
                var agentVersionService = avaiableAgent.Scope.ServiceProvider.GetRequiredService<AgentVersionService>();

                await jobService.SetRunning(job);

                var latestVersion = await agentVersionService.ReadLatest();
                if (latestVersion is null || latestVersion.FileContent is null)
                {
                    throw new RunnerException("Invalid latest AgentVersion to update!");
                }

                var request = new UpdateVersionRequest
                {
                    Version = latestVersion.Version,
                    Content = latestVersion.FileContent
                };

                await _agentHub.Clients.Client(avaiableAgent.ConnectionId).SendAsync("UpdateVersion", request);

                await jobService.SetCompleted(job);
            }
        }

        private async Task RunActionJob(Job job)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                var runService = scope.ServiceProvider.GetRequiredService<RunService>();
                Assert.MustNotNull(job.RunId, "RunActionJob missing RunId, JobId: " + job.JobId);
                Assert.MustNotNull(job.ActionId, "RunActionJob missing ActionId, JobId: " + job.JobId);
                var run = await runService.ReadById(job.RunId.Value);
                Assert.MustNotNull(run, "RunActionJob invalid RunId, JobId: " + job.JobId);

                var control = ActionControl.From(run);
                var action = control.FindAction(job.ActionId.Value);
                Assert.MustNotNull(action, "RunActionJob invalid Action on RunId, JobId: " + job.JobId);

                var scriptService = scope.ServiceProvider.GetRequiredService<ScriptService>();
                var sts = await scriptService.ReadVersionByScriptPath("action.ScriptPath");
                Assert.MustNotNull(sts, $"RunActionJob script not found! ScriptPath: \"{"action.ScriptPath"}\", JobId: {job.JobId}");

                var agentPoolPath = control.ComputeAgentPoolPathForAction(job.ActionId.Value);

                var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
                var agentPoolNode = await nodeService.ReadLocation(agentPoolPath);
                if (agentPoolNode is not null)
                {
                    var agentService = scope.ServiceProvider.GetRequiredService<AgentService>();
                    var agents = await agentService.ReadAgentsForPool(agentPoolNode.NodeId);
                    if (agents.Any())
                    {
                        foreach (var agent in agents)
                        {
                            var agentconnected = FindByAgent(agent.AgentId);
                            if (agentconnected is not null)
                            {
                                var agentTags = agent.RegistredTags;
                                if (agent.ExtraTags is not null)
                                {
                                    agentTags.AddRange(agent.ExtraTags);
                                }

                                var actionTags = action.Tags ?? new List<string>();

                                if (agentTags.Intersect(actionTags).Count() == agentTags.Count)
                                {
                                    agentconnected.RunningJobId = job.JobId;

                                    var runService2 = agentconnected.Scope.ServiceProvider.GetRequiredService<RunService>();
                                    await runService2.SetRunning(job.RunId.Value, job.ActionId.Value);

                                    var jobService2 = agentconnected.Scope.ServiceProvider.GetRequiredService<JobService>();
                                    job.AgentId = agent.AgentId;
                                    await jobService2.SetRunning(job);

                                    var request = new RunScriptRequest
                                    {
                                        ScriptId = sts.Value.Script.ScriptId.ToString(),
                                        Assembly = sts.Value.ScriptVersion.Assembly,  //TODO: pass this data to agent folder
                                        FullTypeName = sts.Value.ScriptVersion.FullTypeName,          //TODO: pass this data to agent folder
                                        Version = sts.Value.ScriptVersion.Version,
                                        Data = new List<Interface.Model.Data.DataProperty>()
                                    };

                                    _ = _agentHub.Clients.Client(agentconnected.ConnectionId).SendAsync("RunScript", request);
                                    
                                    return;
                                }
                            }
                        }

                    }
                }

                await jobService.SetWaiting(job);
            }
        }

        private void OnJobStop(Job job)
        {
            var agentConnect = FindByJobId(job.JobId);
            if (agentConnect != null)
            {
                _agentHub.Clients.Client(agentConnect.ConnectionId).SendAsync("StopScript")
                    .Wait();
            }
        }

        internal async Task ScriptError(string connectionId, ScriptErrorRequest request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect != null)
            {
                //var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();
                //Assert.MustNotNull(agentConnect.RunningJobId, "Missing RunningJobId!");
                //var job = await jobService.ReadById(agentConnect.RunningJobId.Value);
                //Assert.MustNotNull(job, "Job not found! " + agentConnect.RunningJobId.Value);

                //var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
                //await runService.SetError(job.RunId, job.ActionId, request.Error);

                //await jobService.SetError(job);
            }
        }

        internal async Task ScriptFinish(string connectionId, RunScriptResponse request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect != null)
            {
                //var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();
                //Assert.MustNotNull(agentConnect.RunningJobId, "Missing RunningJobId!");
                //var job = await jobService.ReadById(agentConnect.RunningJobId.Value);
                //Assert.MustNotNull(job, "Job not found! " + agentConnect.RunningJobId.Value);

                //var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
                //await runService.SetCompleted(job.RunId, job.ActionId);

                //await jobService.SetCompleted(job);

                //agentConnect.RunningJobId = null;

                _ = CheckJobsForAgent(agentConnect);
            }
        }

        internal async Task ScriptLog(string connectionId, ScriptLogRequest request)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect != null)
            {
                //var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();
                //Assert.MustNotNull(agentConnect.RunningJobId, "Missing RunningJobId!");
                //var job = await jobService.ReadById(agentConnect.RunningJobId.Value);
                //Assert.MustNotNull(job, "Job not found! " + agentConnect.RunningJobId.Value);

                //var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
                //await runService.WriteLog(job.RunId, request.Text);
            }
        }
    }
}
