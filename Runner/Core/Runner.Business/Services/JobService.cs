using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.Actions;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using Runner.Business.WatcherNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Runner.Business.Services
{
    public class JobService : ServiceBase
    {
        private ManualAgentWatcherNotification _manualAgentWatcherNotification;

        public JobService(Database database, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _manualAgentWatcherNotification = (ManualAgentWatcherNotification)agentWatcherNotification;
        }

        public Task<List<Job>> ReadJobs(Agent agent)
        {
            //var sort = Builders<Job>.Sort
            //    .Ascending(j => j.Queued);

            //// trocar torun para state
            //// find and update para preget

            return Job
                .ToListAsync(j => j.AgentId == agent.Id);
        }

        public Task<List<Job>> ReadJobsWaiting()
        {
            //var sort = Builders<Job>.Sort
            //    .Ascending(j => j.Queued);

            //// trocar torun para state
            //// find and update para preget

            return Job
                .ToListAsync(j => j.Status == JobStatus.Waiting);
        }

        public Task<Job?> ReadById(ObjectId jobId)
        {
            //var sort = Builders<Job>.Sort
            //    .Ascending(j => j.Queued);

            //// trocar torun para state
            //// find and update para preget

            return Job
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task CreateJob(Run run, Actions.Action action)
        {
            var job = new Job
            {
                AgentPool = action.AgentPool ?? string.Empty,
                Tags = action.Tags ?? new List<string>(),
                ActionId = action.ActionId,
                RunId = run.Id,
                Queued = DateTime.Now,
                Status = JobStatus.Waiting
            };

            await Job.CreateAsync(job);

            _manualAgentWatcherNotification.InvokeJobCreated(job);
        }

        public async Task StopJob(Run run, Actions.Action action)
        {
            var job = await Job.FirstOrDefaultAsync(j =>
                j.RunId == run.Id
                && j.ActionId == action.ActionId
                && (j.Status == JobStatus.Waiting || j.Status == JobStatus.Running));
            Assert.MustNotNull(job, $"Invalid Job for RunId: {run.Id}, ActionId: {action.ActionId}");

            if (job.Status == JobStatus.Waiting)
            {
                var update = Builders<Job>.Update
                    .Set(j => j.Status, JobStatus.Completed)
                    .Set(j => j.Started, DateTime.Now)
                    .Set(j => j.End, DateTime.Now);

                await Job.UpdateAsync(job, update);
            }
            else
            {
                _manualAgentWatcherNotification.InvokeStopJob(job);
            }
        }

        public Task SetRunning(Job job, ObjectId agentId)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Running)
                .Set(j => j.Started, DateTime.Now)
                .Set(j => j.AgentId, agentId);

            return Job.UpdateAsync(job, update);
        }

        public Task SetError(Job job)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Error)
                .Set(j => j.End, DateTime.Now);

            return Job.UpdateAsync(job, update);
        }

        public Task SetCompleted(Job job)
        {
            var update = Builders<Job>.Update
                .Set(j => j.Status, JobStatus.Completed)
                .Set(j => j.End, DateTime.Now);

            return Job.UpdateAsync(job, update);
        }
    }
}
