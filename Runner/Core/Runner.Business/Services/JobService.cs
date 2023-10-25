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
        private UserLogged _userLogged;
        private IAgentWatcherNotification _agentWatcherNotification;

        public JobService(Database database, UserLogged userLogged, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _userLogged = userLogged;
            _agentWatcherNotification = agentWatcherNotification;
        }

        public Task<Job?> FindNextJobToRun(Agent agent)
        {
            var sort = Builders<Job>.Sort
                .Ascending(j => j.Queued);

            // trocar torun para state
            // find and update para preget

            return Job
                .Find(j =>
                    j.AgentPoolId == agent.Parent &&
                    j.AgentId == agent.Id &&
                    j.ToRun == true)
                .Sort(sort)
                .FirstOrDefaultAsync();
        }

        public async Task CreateJob(Agent agent)
        {
            Assert.MustNotNull(agent.Parent, "Invalid Agent Parent!");

            var job = new Job
            {
                AgentPoolId = agent.Parent.Value,
                AgentId = agent.Id,
                Queued = DateTime.Now,
                ToRun = true
            };

            await Job.CreateAsync(job);

            _agentWatcherNotification.InvokeRunScript(agent);
        }
    }
}
