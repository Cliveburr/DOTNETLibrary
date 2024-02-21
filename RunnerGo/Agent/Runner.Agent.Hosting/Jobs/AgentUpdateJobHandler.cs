using Runner.Business.Jobs;
using Runner.Business.Services;
using Runner.Business.Entities.Job;
using Runner.Agent.Interface.Model;
using Runner.Agent.Hosting.Services;

namespace Runner.Agent.Hosting.Jobs
{
    public class AgentUpdateJobHandler : IJobHandler
    {
        private readonly AgentVersionService _agentVersionService;
        private readonly AgentManagerService _agentManagerService;

        public AgentUpdateJobHandler(AgentVersionService agentVersionService, AgentManagerService agentManagerService)
        {
            _agentVersionService = agentVersionService;
            _agentManagerService = agentManagerService;
        }

        public async Task<bool> Execute(Job job)
        {
            var agentConnected = _agentManagerService
                .FindAgent(a => a.JobRunning is null && a.AgentId == job.AgentId);
            if (agentConnected is null)
            {
                return false;
            }

            var latestVersion = await _agentVersionService.ReadLatest();
            if (latestVersion is null || latestVersion.FileContent is null)
            {
                throw new RunnerException("Invalid latest AgentVersion to update!");
            }

            await _agentManagerService.CallUpdateVersion("UpdateVersion", new UpdateVersionRequest
            {
                Version = latestVersion.Version,
                Content = latestVersion.FileContent
            });

            return true;
        }
    }
}
