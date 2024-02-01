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

        public async Task QueueAgentUpdate(ObjectId agentId)
        {
            var job = new Job
            {
                Type = JobType.AgentUpdate,
                Status = JobStatus.Queued,
                Queued = DateTime.UtcNow,

                AgentId = agentId
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobQueued(job);
        }

        public async Task QueueRunAction(int actionId, ObjectId runId)
        {
            var job = new Job
            {
                Type = JobType.RunAction,
                Status = JobStatus.Queued,
                Queued = DateTime.Now,

                ActionId = actionId,
                RunId = runId,
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobQueued(job);
        }

        public async Task<Job> QueueExtractScriptPackage(ObjectId scriptContentId, ObjectId scriptPackageId)
        {
            var job = new Job
            {
                Type = JobType.ExtractScriptPackage,
                Status = JobStatus.Queued,
                Queued = DateTime.UtcNow,

                ScriptContentId = scriptContentId,
                ScriptPackageId = scriptPackageId
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobQueued(job);

            return job;
        }

        public Task<Job?> GetWaitingAndQueueOfTypes(JobType[] types)
        {
            var sort = Builders<Job>.Sort
                .Ascending(j => j.Queued);

            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Queued);

            var options = new FindOneAndUpdateOptions<Job, Job?>
            {
                ReturnDocument = ReturnDocument.After,
                Sort = sort
            };

            return Job.Collection.FindOneAndUpdateAsync<Job, Job?>(j =>
                    j.Status == JobStatus.Waiting &&
                    types.Contains(j.Type),
                    update,
                    options);
        }

        public Task<Job?> GetWaitingAndQueueExtractScript()
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Queued);

            var options = new FindOneAndUpdateOptions<Job, Job?>
            {
                ReturnDocument = ReturnDocument.After
            };

            return Job.Collection.FindOneAndUpdateAsync<Job, Job?>(j =>
                    j.Status == JobStatus.Waiting &&
                    j.Type == JobType.ExtractScriptPackage,
                    update,
                    options);
        }

        public Task SetWaiting(Job job)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Waiting);

            return Job
                .UpdateAsync(j => j.JobId == job.JobId, update);
        }

        public Task SetRunning(Job job)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Running)
                .Set(j => j.Started, DateTime.UtcNow)
                .Set(j => j.AgentId, job.AgentId);

            return Job
                .UpdateAsync(j => j.JobId == job.JobId, update);
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
                    _manualAgentWatcherNotification?.InvokeJobStop(job);
                }
            }
        }
    }
}
