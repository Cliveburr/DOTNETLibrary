using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;

namespace Runner.Business.Services
{
    public class JobService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;
        private ManualAgentWatcherNotification? _manualAgentWatcherNotification;

        public JobService(Database database, IdentityProvider identityProvider, NodeService nodeService, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
            _manualAgentWatcherNotification = agentWatcherNotification as ManualAgentWatcherNotification;
        }

        public Task<List<Job>> ReadForAgent(ObjectId agentId)
        {
            return Job
                .ToListAsync(j => j.AgentId == agentId);
        }

        public Task<List<Job>> Read()
        {
            return Job
                .ToListAsync();
        }

        public async Task AddAgentUpdate(ObjectId agentId)
        {
            var job = new Job
            {
                Type = JobType.AgentUpdate,
                Status = JobStatus.Waiting,
                Queued = DateTime.UtcNow,

                AgentId = agentId
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobCreated(job);
        }

        public async Task AddRunAction(int actionId, ObjectId runId)
        {
            var job = new Job
            {
                Type = JobType.RunAction,
                Status = JobStatus.Waiting,
                Queued = DateTime.Now,

                ActionId = actionId,
                RunId = runId,
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobCreated(job);
        }

        public async Task<Job> AddExtractScriptPackage(ObjectId scriptContentId, ObjectId scriptPackageId)
        {
            var job = new Job
            {
                Type = JobType.ExtractScriptPackage,
                Status = JobStatus.Waiting,
                Queued = DateTime.UtcNow,

                ScriptContentId = scriptContentId,
                ScriptPackageId = scriptPackageId
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobCreated(job);

            return job;
        }

        public Task<List<Job>> ReadJobsWaitingOfTypes(JobType[] types)
        {
            //var sort = Builders<Job>.Sort
            //    .Ascending(j => j.Queued);

            //// trocar torun para state
            //// find and update para preget

            return Job
                .ToListAsync(j =>
                    j.Status == JobStatus.Waiting &&
                    types.Contains(j.Type));
        }

        public Task SetRunning(Job job)
        {
            job.Status = JobStatus.Running;
            job.Started = DateTime.UtcNow;

            return Job
                .ReplaceAsync(j => j.JobId == job.JobId, job);
        }

        public Task SetError(Job job, Exception ex)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Error)
                .Set(j => j.ErrorMessage, ex.ToString())
                .Set(j => j.End, DateTime.Now);

            return Job
                .UpdateAsync(j => j.JobId == job.JobId, update);
        }

        public Task SetCompleted(Job job)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Completed)
                .Set(j => j.End, DateTime.Now);

            return Job.UpdateAsync(j => j.JobId == job.JobId, update);
        }

        public async Task StopJob(Run run, Actions.Action action)
        {
            var job = await Job.FirstOrDefaultAsync(j =>
                j.RunId == run.RunId
                && j.ActionId == action.ActionId
                && (j.Status == JobStatus.Waiting || j.Status == JobStatus.Running));
            if (job is not null)
            {
                if (job.Status == JobStatus.Waiting)
                {
                    var update = Builders<Job>.Update
                        .Set(j => j.Status, JobStatus.Completed)
                        .Set(j => j.Started, DateTime.Now)
                        .Set(j => j.End, DateTime.Now);

                    await Job
                        .UpdateAsync(j => j.JobId == job.JobId, update);
                }
                else
                {
                    _manualAgentWatcherNotification?.InvokeStopJob(job);
                }
            }
        }
    }
}
