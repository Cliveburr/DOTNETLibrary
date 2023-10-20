using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Connections;
using Runner.Agent.Hosting.Hubs;
using Runner.Agent.Hosting.Model.AgentManager;
using Runner.Business.Authentication;
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

namespace Runner.Agent.Hosting.Services
{
    public class AgentManagerService
    {
        private readonly IHubContext<AgentHub> _agentHub;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<AgentStorage> _agents;
        private readonly IAgentWatcherNotification _agentWatcherNotification;

        private class AgentStorage
        {
            public required string ConnectionId { get; set; }
            public required Business.Entities.Agent.Agent Agent { get; set; }
            public required IServiceScope Scope { get; set; }
        }

        public AgentManagerService(IHubContext<AgentHub> agentHub, IServiceProvider serviceProvider, IAgentWatcherNotification agentWatcherNotification)
        {
            _agentHub = agentHub;
            _serviceProvider = serviceProvider;
            _agents = new List<AgentStorage>();
            _agentWatcherNotification = agentWatcherNotification;

            _agentWatcherNotification.OnRunScript += OnRunScript;
        }

        internal async Task Register(string connectionId, RegisterRequest request)
        {
            var scope = _serviceProvider.CreateScope();

            var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
            if (!(await authenticationService.CheckAccessToken(request.AccessToken, AccessTokenType.WebUI))) //TODO, tropara para Agent
            {
                throw new AuthenticationException();
            }

            var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
            var agent = await nodeService.RegisterAgent(request.AgentPool, request.MachineName, request.Tags);
            if (agent == null)
            {
                throw new Exception("Agent register invalid!");
            }

            var found = FindStorage(agent);
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
                _agents.Add(new AgentStorage
                {
                    ConnectionId = connectionId,
                    Agent = agent,
                    Scope = scope
                });
            }

            var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
            var nextJobToRun = await jobService.FindNextJobToRun(agent);
            if (nextJobToRun != null)
            {
                await RunScript(agent);
            }
        }

        private AgentStorage? FindStorage(string connectionId)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.ConnectionId == connectionId);
            }
        }

        private AgentStorage? FindStorage(Business.Entities.Agent.Agent agent)
        {
            lock (_agents)
            {
                return _agents
                    .FirstOrDefault(a => a.Agent.Id == agent.Id);
            }
        }

        internal async Task Heartbeat(string connectionId)
        {
            var storage = FindStorage(connectionId);
            if (storage == null)
            {
                return;
            }

            var nodeService = storage.Scope.ServiceProvider.GetRequiredService<NodeService>();
            storage.Agent = await nodeService.AgentHeartbeat(storage.Agent.Id);
        }

        internal async Task Offline(string connectionId)
        {
            var storage = FindStorage(connectionId);
            if (storage == null)
            {
                return;
            }

            var nodeService = storage.Scope.ServiceProvider.GetRequiredService<NodeService>();
            _ = await nodeService.UpdateAgentStatus(storage.Agent.Id, AgentStatus.Offline);

            try
            {
                storage.Scope?.Dispose();
            }
            catch { }

            lock (_agents)
            {
                _agents.Remove(storage);
            }
        }

        private void OnRunScript(Business.Entities.Agent.Agent agent)
        {
            _ = RunScript(agent);
        }

        private Task RunScript(Business.Entities.Agent.Agent agent)
        {
            return Task.Run(() =>
            {
                var storage = FindStorage(agent);
                if (storage == null)
                {
                    return;
                }

                _agentHub.Clients.Client(storage.ConnectionId).SendAsync("RunScript").Wait(); ;
            });
        }
    }
}
