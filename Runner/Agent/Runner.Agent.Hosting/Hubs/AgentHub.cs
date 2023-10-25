using Microsoft.AspNetCore.SignalR;
using Runner.Agent.Hosting.Model.AgentManager;
using Runner.Agent.Hosting.Services;
using Runner.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Hosting.Hubs
{
    public class AgentHub : Hub
    {
        private readonly AgentManagerService _agentManagerService;

        public AgentHub(AgentManagerService agentManager)
        {
            _agentManagerService = agentManager;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return _agentManagerService.Offline(Context.ConnectionId);
        }

        public Task Register(RegisterRequest request)
        {
            return _agentManagerService.Register(Context.ConnectionId, request);
        }

        public Task Heartbeat()
        {
            return _agentManagerService.Heartbeat(Context.ConnectionId);
        }

        public Task ScriptStarted()
        {
            return Task.CompletedTask;
        }

        public Task ScriptError()
        {
            return Task.CompletedTask;
        }

        public Task ScriptFinish()
        {
            return Task.CompletedTask;
        }
    }
}
