using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Model.Schedule;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;

namespace Runner.Business.Services
{
    public class JobScheduleService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;
        private ManualAgentWatcherNotification? _manualAgentWatcherNotification;

        public JobScheduleService(Database database, IdentityProvider identityProvider, NodeService nodeService, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
            _manualAgentWatcherNotification = agentWatcherNotification as ManualAgentWatcherNotification;
        }

        public Task<List<FlowScheduleList>> ReadFlowScheduleList(ObjectId flowNodeId)
        {
            var query = from n in Node.AsQueryable()
                        join fs in FlowSchedule.AsQueryable() on n.NodeId equals fs.NodeId
                        join js in JobSchedule.AsQueryable() on fs.JobScheduleId equals js.JobScheduleId
                        where n.ParentId == flowNodeId
                        select new FlowScheduleList
                        {
                            FlowScheduleId = fs.FlowScheduleId,
                            JobSchedule = js
                        };

            return query
                .ToListAsync();
        }

        public IQueryable<JobSchedule> FindActiveMissingTickers()
        {
            var query = from js in JobSchedule.AsQueryable()
                        join jt in JobTicker.AsQueryable() on js.JobScheduleId equals jt.JobScheduleId into j
                        where js.Active && !j.Any()
                        select js;

            return query;
        }

        public IQueryable<JobTickerAndSchedule> FindExpiredTickers(DateTime now)
        {
            var query = from jt in JobTicker.AsQueryable()
                        join js in JobSchedule.AsQueryable() on jt.JobScheduleId equals js.JobScheduleId into jss
                        where jt.TargetUtc <= now
                        orderby jt.TargetUtc ascending
                        select new JobTickerAndSchedule
                        {
                            Ticker = jt,
                            Schedule = jss.FirstOrDefault()
                        };

            return query;
        }

        public async Task<JobTicker?> FindClosestTickerToExpire()
        {
            var sort = Builders<JobTicker>.Sort
                .Ascending(jt => jt.TargetUtc);

            return await JobTicker.Collection
                .Find(Builders<JobTicker>.Filter.Empty)
                .Sort(sort)
                .FirstOrDefaultAsync();
        }

        public Task DeleteTicker(ObjectId jobTickerId)
        {
            return JobTicker
                .DeleteAsync(jt => jt.JobTickerId == jobTickerId);
        }

        public Task DeleteTickerByJobScheduleId(ObjectId jobScheduleId)
        {
            return JobTicker
                .DeleteAsync(jt => jt.JobScheduleId == jobScheduleId);
        }

        public async Task CreateJob(JobSchedule schedule)
        {
            switch (schedule.JobType)
            {
                default:
                    await DeactiveJobSchedule(schedule);
                    break;
            }
        }

        private Task DeactiveJobSchedule(JobSchedule schedule)
        {
            var update = Builders<JobSchedule>.Update
                .Set(js => js.Active, false);

            return JobSchedule
                .UpdateAsync(js => js.JobScheduleId == schedule.JobScheduleId, update);
        }

        public Task CreateTicker(ObjectId jobScheduleId, DateTime targetUtc)
        {
            var jobticker = new JobTicker
            {
                JobScheduleId = jobScheduleId,
                TargetUtc = targetUtc
            };

            return JobTicker
                .InsertAsync(jobticker);
        }


    }
}
