using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver.Core.Connections;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Model.AgentManager;
using Runner.Business.Actions;
using Runner.Business.Authentication;
using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using Runner.Business.Entities.Identity;
using Runner.Business.Services;
using Runner.Business.WatcherNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Runner.Agent.Hosting.Services
{
    public class AgentManagerService
    {
        private readonly IHubContext<AgentHub> _agentHub;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AgentConnect> _agents;
        private readonly IAgentWatcherNotification _agentWatcherNotification;

        private class AgentConnect
        {
            public required string ConnectionId { get; set; }
            public required string AgentPool { get; set; }
            public required ObjectId AgentId { get; set; }
            public required IServiceScope Scope { get; set; }
        }

        public AgentManagerService(IHubContext<AgentHub> agentHub, IServiceProvider serviceProvider, IAgentWatcherNotification agentWatcherNotification)
        {
            _agentHub = agentHub;
            _serviceProvider = serviceProvider;
            _agents = new List<AgentConnect>();
            _agentWatcherNotification = agentWatcherNotification;

            _agentWatcherNotification.OnJobCreated += OnJobCreated;
        }

        internal async Task Register(string connectionId, RegisterRequest request)
        {
            var scope = _serviceProvider.CreateScope();

            var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
            if (!(await authenticationService.CheckAccessToken(request.AccessToken, AccessTokenType.WebUI))) //TODO, trocar para tipo Agent
            {
                throw new AuthenticationException();
            }

            var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
            var agent = await nodeService.RegisterAgent(request.AgentPool, request.MachineName, request.Tags);

            var found = FindConnected(agent);
            if (found != null)
            {
                try
                {
                    found.Scope?.Dispose();
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
                    ConnectionId = connectionId,
                    AgentPool = request.AgentPool,
                    AgentId = agent.Id,
                    Scope = scope
                });
            }

            _ = CheckAgentForJobs();
        }

        private AgentConnect? FindConnected(string connectionId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.ConnectionId == connectionId);
            }
        }

        private AgentConnect? FindConnected(Business.Entities.Agent.Agent agent)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.AgentId == agent.Id);
            }
        }

        internal async Task Heartbeat(string connectionId)
        {
            var storage = FindConnected(connectionId);
            if (storage == null)
            {
                return;
            }

            var nodeService = storage.Scope.ServiceProvider.GetRequiredService<NodeService>();
            await nodeService.AgentHeartbeat(storage.AgentId);
        }

        internal async Task Offline(string connectionId)
        {
            var agentConnect = FindConnected(connectionId);
            if (agentConnect != null)
            {
                var nodeService = agentConnect.Scope.ServiceProvider.GetRequiredService<NodeService>();
                await nodeService.UpdateAgentOffline(agentConnect.AgentId);
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

            foreach (var job in jobs)
            {
                var jobNormalizedAgentPool = NormalizeAgentPool(job.AgentPool);
                var agentsAvaliable = new List<(Business.Entities.Agent.Agent Agent, NodeService NodeService, AgentConnect AgentConnect)>();

                foreach (var agentConnected in _agents)
                {
                    var agentNormalizedAgentPool = NormalizeAgentPool(agentConnected.AgentPool);

                    if (jobNormalizedAgentPool == agentNormalizedAgentPool)
                    {
                        var nodeService = agentConnected.Scope.ServiceProvider.GetRequiredService<NodeService>();

                        var agentPool = await nodeService.ReadLocation(agentConnected.AgentPool) as AgentPool;
                        if (agentPool != null && agentPool.Enabled)
                        {
                            var agent = await nodeService.ReadById(agentConnected.AgentId) as Business.Entities.Agent.Agent;
                            if (agent != null && agent.Enabled && agent.Status == AgentStatus.Idle)
                            {
                                agentsAvaliable.Add((agent, nodeService, agentConnected));
                            }
                        }
                    }
                }

                if (agentsAvaliable.Any())
                {
                    var agentsWithTags = new List<(Business.Entities.Agent.Agent Agent, NodeService NodeService, AgentConnect AgentConnect)>();

                    foreach (var agentAvaliable in agentsAvaliable)
                    {
                        var run = await agentAvaliable.NodeService.ReadById(job.RunId) as Run;
                        if (run != null)
                        {
                            var action = run.Actions
                                .FirstOrDefault(a => a.ActionId == job.ActionId);
                            if (action != null)
                            {
                                var agentTags = agentAvaliable.Agent.RegistredTags;
                                if (agentAvaliable.Agent.ExtraTags != null)
                                {
                                    agentTags.AddRange(agentAvaliable.Agent.ExtraTags);
                                }

                                var actionTags = action.Tags;

                                if (agentTags.Intersect(actionTags).Count() == agentTags.Count)
                                {
                                    agentsWithTags.Add(agentAvaliable);
                                }
                            }
                        }
                    }

                    if (agentsWithTags.Any())
                    {
                        var chooseAgent = agentsWithTags
                            .OrderBy(a => a.Agent.LastExecuted)
                            .First();

                        binds.Add(new JobAgentBind
                        {
                            Job = job,
                            AgentConnect = chooseAgent.AgentConnect
                        });
                    }
                }

                //TODO: checar se job deu timeout
            }

            return binds;
        }

        private class JobAgentBind
        {
            public required Job Job { get; set; }
            public required AgentConnect AgentConnect { get; set; }
        }

        private async Task CheckAgentForJobs()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

                var jobsWaiting = await jobService.ReadJobsWaiting();

                var jobWithAgents = await MixJobWithAgents(jobsWaiting);

                foreach (var jobWithAgent in jobWithAgents)
                {
                    try
                    {
                        await _agentHub.Clients.Client(jobWithAgent.AgentConnect.ConnectionId).SendAsync("RunScript");
                    }
                    catch (Exception ex)
                    {
                        //todo:
                    }
                }
            }
        }

        internal Task ScriptStarted(string connectionId)
        {
            throw new NotImplementedException();
        }

        internal Task ScriptError(string connectionId, ScriptErrorRequest request)
        {
            throw new NotImplementedException();
        }

        internal Task ScriptFinish(string connectionId, ScriptFinishRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
