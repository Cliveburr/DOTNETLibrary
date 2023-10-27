using MongoDB.Bson;
using MongoDB.Driver;
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
        private IAgentWatcherNotification _agentWatcherNotification;

        public JobService(Database database, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _agentWatcherNotification = agentWatcherNotification;
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

        public async Task CreateJob(Run run, Actions.Action action)
        {
            var job = new Job
            {
                AgentPool = action.AgentPool,
                Tags = action.Tags,
                ActionId = action.ActionId,
                RunId = run.Id,
                Queued = DateTime.Now,
                Status = JobStatus.Waiting
            };

            await Job.CreateAsync(job);

            _agentWatcherNotification.InvokeJobCreated(job);
        }
    }
}
