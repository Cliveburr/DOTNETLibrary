using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Interface.Model;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Security;
using Runner.Business.Security;
using Runner.Business.Services.NodeTypes;
using Runner.Business.WatcherNotification;
using System.Security.Authentication;

namespace Runner.Agent.Hosting.Services
{
    public class AgentManagerService : IDisposable
    {
        private readonly IHubContext<AgentHub> _agentHub;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AgentConnect> _agents;
        private readonly IAgentWatcherNotification _agentWatcherNotification;

        private class AgentConnect
        {
            public required string ConnectionId { get; set; }
            public required AgentHub Hub { get;set; }
            public required ObjectId AgentPoolNodeId { get; set; }
            public required ObjectId AgentNodeId { get; set; }
            public required IServiceScope Scope { get; set; }
            public ObjectId? RunningJobId { get; set; }
        }

        public AgentManagerService(IHubContext<AgentHub> agentHub, IServiceProvider serviceProvider, IAgentWatcherNotification agentWatcherNotification)
        {
            _agentHub = agentHub;
            _serviceProvider = serviceProvider;
            _agents = new List<AgentConnect>();
            _agentWatcherNotification = agentWatcherNotification;

            _agentWatcherNotification.OnJobCreated += OnJobCreated;
            _agentWatcherNotification.OnStopJob += OnStopJob;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobCreated -= OnJobCreated;
            _agentWatcherNotification.OnStopJob -= OnStopJob;
        }

        internal async Task Register(AgentHub hub, RegisterRequest request)
        {
            var scope = _serviceProvider.CreateScope();

            try
            {
                var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
                if (!(await authenticationService.LoginByAccessToken(request.AccessToken, AccessTokenType.WebUI))) //TODO, trocar para tipo Agent
                {
                    throw new AuthenticationException();
                }

                var agentService = scope.ServiceProvider.GetRequiredService<AgentService>();
                var (agentPoolNodeId, agentNodeId) = await agentService.Register(request.AgentPoolPath, request.MachineName, request.Tags);

                var found = FindByAgent(agentNodeId);
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
                    _agents.Add(new AgentConnect
                    {
                        ConnectionId = hub.Context.ConnectionId,
                        Hub = hub,
                        AgentPoolNodeId = agentPoolNodeId,
                        AgentNodeId = agentNodeId,
                        Scope = scope
                    });
                }
            }
            catch
            {
                try
                {
                    scope.Dispose();
                }
                catch { }
                throw;
            }

            _ = CheckAgentForJobs();
        }

        private AgentConnect? FindByAgent(ObjectId agentNodeId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.AgentNodeId == agentNodeId);
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

        //private AgentConnect? FindConnected(Business.Entities.Node.Agent.Agent agent)
        //{
        //    lock (_agents)
        //    {
        //        return _agents
        //            .FirstOrDefault(a => a.AgentId == agent.Id);
        //    }
        //}

        //private AgentConnect? FindConnected(Job job)
        //{
        //    lock (_agents)
        //    {
        //        return _agents
        //            .FirstOrDefault(a => a.RunningJobId == job.Id);
        //    }
        //}

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
                await agentService.UpdateOffline(agentConnect.AgentNodeId);
                
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

        private void OnJobCreated(Job job)
        {
            _ = CheckAgentForJobs();
        }

        private string NormalizeAgentPool(string agentPool)
        {
            return agentPool
                .Trim('/')
                .ToLower();
        }

        private async Task<List<JobAgentBind>> MixJobWithAgents(List<Job> jobs)
        {
            var binds = new List<JobAgentBind>();

            //foreach (var job in jobs)
            //{
            //    var jobNormalizedAgentPool = NormalizeAgentPool(job.AgentPool);
            //    var agentsAvaliable = new List<(Business.Entities.Node.Agent.Agent Agent, NodeService NodeService, AgentConnect AgentConnect)>();

            //    foreach (var agentConnected in _agents)
            //    {
            //        if (agentConnected.RunningJobId.HasValue)
            //        {
            //            continue;
            //        }

            //        var agentNormalizedAgentPool = NormalizeAgentPool(agentConnected.AgentPool);

            //        if (jobNormalizedAgentPool == agentNormalizedAgentPool)
            //        {
            //            var nodeService = agentConnected.Scope.ServiceProvider.GetRequiredService<NodeService>();

            //            var agentPool = await nodeService.ReadLocation(agentConnected.AgentPool) as AgentPool;
            //            if (agentPool != null && agentPool.Enabled)
            //            {
            //                var agent = await nodeService.ReadById(agentConnected.AgentId) as Business.Entities.Node.Agent.Agent;
            //                if (agent != null && agent.Enabled && agent.Status == AgentStatus.Idle)
            //                {
            //                    agentsAvaliable.Add((agent, nodeService, agentConnected));
            //                }
            //            }
            //        }
            //    }

            //    if (agentsAvaliable.Any())
            //    {
            //        var agentsWithTags = new List<(Business.Entities.Node.Agent.Agent Agent, NodeService NodeService, AgentConnect AgentConnect)>();

            //        foreach (var agentAvaliable in agentsAvaliable)
            //        {
            //            var run = await agentAvaliable.NodeService.ReadById(job.RunId) as Run;
            //            if (run != null)
            //            {
            //                var action = run.Actions
            //                    .FirstOrDefault(a => a.ActionId == job.ActionId);
            //                if (action != null)
            //                {
            //                    var agentTags = agentAvaliable.Agent.RegistredTags;
            //                    if (agentAvaliable.Agent.ExtraTags != null)
            //                    {
            //                        agentTags.AddRange(agentAvaliable.Agent.ExtraTags);
            //                    }

            //                    var actionTags = action.Tags ?? new List<string>();

            //                    if (agentTags.Intersect(actionTags).Count() == agentTags.Count)
            //                    {
            //                        agentsWithTags.Add(agentAvaliable);
            //                    }
            //                }
            //            }
            //        }

            //        if (agentsWithTags.Any())
            //        {
            //            var chooseAgent = agentsWithTags
            //                .OrderBy(a => a.Agent.LastExecuted)
            //                .First();

            //            binds.Add(new JobAgentBind
            //            {
            //                Job = job,
            //                AgentConnect = chooseAgent.AgentConnect
            //            });
            //        }
            //    }

            //    //TODO: checar se job deu timeout
            //}

            return binds;
        }

        private class JobAgentBind
        {
            public required Job Job { get; set; }
            public required AgentConnect AgentConnect { get; set; }
        }

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private bool _justOneWait = false;

        private async Task CheckAgentForJobs()
        {
            if (_justOneWait)
            {
                return;
            }
            else
            {
                _justOneWait = true;
            }

            await _semaphoreSlim.WaitAsync();

            _justOneWait = false;

            try
            {
                //    using (var scope = _serviceProvider.CreateScope())
                //    {
                //        var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

                //        var jobsWaiting = await jobService.ReadJobsWaiting();

                //        var jobWithAgents = await MixJobWithAgents(jobsWaiting);

                //        foreach (var jobWithAgent in jobWithAgents)
                //        {
                //            try
                //            {
                //                var request = new RunScriptRequest
                //                {
                //                    Id = jobWithAgent.Job.Id.ToString(),
                //                    Assembly = "",
                //                    Type = "",
                //                    Version = 1,
                //                    Input = new Dictionary<string, object?>()
                //                };

                //                await _agentHub.Clients.Client(jobWithAgent.AgentConnect.ConnectionId).SendAsync("RunScript", request);

                //                jobWithAgent.AgentConnect.RunningJobId = jobWithAgent.Job.Id;
                //            }
                //            catch (Exception ex)
                //            {
                //                //todo:
                //            }
                //        }
                //    }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private void OnStopJob(Job job)
        {
            //var agentConnect = FindConnected(job);
            //if (agentConnect != null)
            //{
            //    _agentHub.Clients.Client(agentConnect.ConnectionId).SendAsync("StopScript")
            //        .Wait();
            //}
        }

        internal async Task ScriptStarted(string connectionId)
        {
            var agentConnect = FindByConnectionId(connectionId);
            if (agentConnect != null)
            {
                //var jobService = agentConnect.Scope.ServiceProvider.GetRequiredService<JobService>();
                //Assert.MustNotNull(agentConnect.RunningJobId, "Missing RunningJobId!");
                //var job = await jobService.ReadById(agentConnect.RunningJobId.Value);
                //Assert.MustNotNull(job, "Job not found! " + agentConnect.RunningJobId.Value);

                //var runService = agentConnect.Scope.ServiceProvider.GetRequiredService<RunService>();
                //await runService.SetRunning(job.RunId, job.ActionId);

                //await jobService.SetRunning(job, agentConnect.AgentId);
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

        internal async Task ScriptFinish(string connectionId, ScriptFinishRequest request)
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
            }

            _ = CheckAgentForJobs();
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
