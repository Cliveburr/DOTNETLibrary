using Runner.Business.Jobs;
using Runner.Business.Entities.Job;
using Runner.Agent.Hosting.Services;

namespace Runner.Agent.Hosting.Jobs
{
    public class StopScriptJobHandler : IJobHandler
    {
        private readonly AgentManagerService _agentManagerService;

        public StopScriptJobHandler(AgentManagerService agentManagerService)
        {
            _agentManagerService = agentManagerService;
        }

        public async Task<bool> Execute(Job job)
        {
            var agentConnect = _agentManagerService
                .FindAgent(a => a.JobRunning?.RunId == job.RunId &&
                    a.JobRunning?.ActionId == job.ActionId);
            if (agentConnect is null)
            {
                return false;
            }

            await _agentManagerService.CallStopScript(agentConnect.ConnectionId);

            return true;
        }
    }
}
