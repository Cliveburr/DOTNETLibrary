using Runner.Business.Entities.Job;
using Runner.Business.Services.NodeTypes;

namespace Runner.Business.Jobs
{
    public class CreateRunJobHandler : IJobHandler
    {
        private readonly RunService _runService;

        public CreateRunJobHandler(RunService runService)
        {
            _runService = runService;
        }

        public async Task<bool> Execute(Job job)
        {
            Assert.MustNotNull(job.FlowId, "CreateRunJobHandler missing FlowId, JobId: " + job.JobId);

            await _runService.CreateRun(job.FlowId.Value, job.RunInput, true);

            return true;
        }
    }
}
