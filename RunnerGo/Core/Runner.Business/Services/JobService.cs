using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Model.Table;
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

        public Task<List<Job>> ReadForAgent(TableRequest request, ObjectId agentId)
        {
            var sort = Builders<Job>.Sort
                .Descending(r => r.Queued);

            var filter = Builders<Job>.Filter
                .Eq(j => j.AgentId, agentId);

            return Job.Collection
                .Find(filter)
                .Sort(sort)
                .Skip(request.Skip)
                .Limit(request.Take)
                .ToListAsync();
        }

        public Task<List<Job>> ReadTable(TableRequest request)
        {
            var sort = Builders<Job>.Sort
                .Descending(r => r.Queued);

            return Job.Collection
                .Find(Builders<Job>.Filter.Empty)
                .Sort(sort)
                .Skip(request.Skip)
                .Limit(request.Take)
                .ToListAsync();
        }

        public async Task Delete(ObjectId jobId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await Job
                .DeleteAsync(j => j.JobId == jobId);
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

        public async Task QueueRunScript(int actionId, ObjectId runId)
        {
            var job = new Job
            {
                Type = JobType.RunScript,
                Status = JobStatus.Queued,
                Queued = DateTime.Now,

                ActionId = actionId,
                RunId = runId,
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobQueued(job);
        }

        public async Task QueueStopScript(int actionId, ObjectId runId)
        {
            var job = new Job
            {
                Type = JobType.StopScript,
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

        public Task<Job?> GetWaiting()
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
                    j.Status == JobStatus.Waiting,
                    update,
                    options);
        }

        //public Task<Job?> GetWaitingAndQueueOfTypes(JobType[] types)
        //{
        //    var sort = Builders<Job>.Sort
        //        .Ascending(j => j.Queued);

        //    var update = Builders<Job>.Update
        //        .Set(j => j.Status, JobStatus.Queued);

        //    var options = new FindOneAndUpdateOptions<Job, Job?>
        //    {
        //        ReturnDocument = ReturnDocument.After,
        //        Sort = sort
        //    };

        //    return Job.Collection.FindOneAndUpdateAsync<Job, Job?>(j =>
        //            j.Status == JobStatus.Waiting &&
        //            types.Contains(j.Type),
        //            update,
        //            options);
        //}

        //public Task<Job?> GetWaitingAndQueueExtractScript()
        //{
        //    var update = Builders<Job>.Update
        //        .Set(j => j.Status, JobStatus.Queued);

        //    var options = new FindOneAndUpdateOptions<Job, Job?>
        //    {
        //        ReturnDocument = ReturnDocument.After
        //    };

        //    return Job.Collection.FindOneAndUpdateAsync<Job, Job?>(j =>
        //            j.Status == JobStatus.Waiting &&
        //            j.Type == JobType.ExtractScriptPackage,
        //            update,
        //            options);
        //}

        public Task SetWaiting(ObjectId jobId)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Waiting);

            return Job
                .UpdateAsync(j => j.JobId == jobId, update);
        }

        public Task SetRunning(ObjectId jobId, ObjectId? agendId = null)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Running)
                .Set(j => j.Started, DateTime.UtcNow)
                .Set(j => j.AgentId, agendId);

            return Job
                .UpdateAsync(j => j.JobId == jobId, update);
        }

        public Task SetError(ObjectId jobId, Exception ex)
        {
            return SetError(jobId, ex.ToString());
        }

        public Task SetError(ObjectId jobId, string errorMessage)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Error)
                .Set(j => j.ErrorMessage, errorMessage)
                .Set(j => j.End, DateTime.Now);

            return Job
                .UpdateAsync(j => j.JobId == jobId, update);
        }

        public Task SetCompleted(ObjectId jobId)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Completed)
                .Set(j => j.End, DateTime.Now);

            return Job.UpdateAsync(j => j.JobId == jobId, update);
        }

        //public async Task StopJob(Run run, Actions.Action action)
        //{
        //    var job = await Job.FirstOrDefaultAsync(j =>
        //        j.RunId == run.RunId
        //        && j.ActionId == action.ActionId
        //        && (j.Status == JobStatus.Waiting || j.Status == JobStatus.Running));
        //    if (job is not null)
        //    {
        //        if (job.Status == JobStatus.Waiting)
        //        {
        //            var update = Builders<Job>.Update
        //                .Set(j => j.Status, JobStatus.Completed)
        //                .Set(j => j.Started, DateTime.Now)
        //                .Set(j => j.End, DateTime.Now);

        //            await Job
        //                .UpdateAsync(j => j.JobId == job.JobId, update);
        //        }
        //        else
        //        {
        //            _manualAgentWatcherNotification?.InvokeJobStop(job);
        //        }
        //    }
        //}
    }
}
