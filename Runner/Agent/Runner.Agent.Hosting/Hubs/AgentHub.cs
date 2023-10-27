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
            return _agentManagerService.ScriptStarted(Context.ConnectionId);
        }

        public Task ScriptError(ScriptErrorRequest request)
        {
            return _agentManagerService.ScriptError(Context.ConnectionId, request);
        }

        public Task ScriptFinish(ScriptFinishRequest request)
        {
            return _agentManagerService.ScriptFinish(Context.ConnectionId, request);
        }

        public Task ScriptLog()
        {
            return Task.CompletedTask;
        }
    }
}
