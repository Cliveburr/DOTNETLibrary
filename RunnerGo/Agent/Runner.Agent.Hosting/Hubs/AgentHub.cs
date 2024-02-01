using Microsoft.AspNetCore.SignalR;
using Runner.Agent.Hosting.Services;
using Runner.Agent.Interface.Model;

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
            return _agentManagerService.Register(this, request);
        }

        public Task Heartbeat()
        {
            return _agentManagerService.Heartbeat();
        }

        public Task ScriptError(ScriptErrorRequest request)
        {
            return _agentManagerService.ScriptError(Context.ConnectionId, request);
        }

        public Task ScriptFinish(RunScriptResponse request)
        {
            return _agentManagerService.ScriptFinish(Context.ConnectionId, request);
        }

        public Task ScriptLog(ScriptLogRequest request)
        {
            return _agentManagerService.ScriptLog(Context.ConnectionId, request);
        }
    }
}
