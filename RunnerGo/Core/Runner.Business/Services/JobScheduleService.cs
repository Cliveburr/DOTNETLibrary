using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes;
using Runner.Business.Model.Schedule;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;
using System.Collections.Generic;
using System.Xml.Linq;

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
                        where n.ParentId == flowNodeId && n.Type == NodeType.FlowSchedule
                        select new FlowScheduleList
                        {
                            FlowSchedule = fs,
                            JobSchedule = js
                        };

            return query
                .ToListAsync();
        }

        public async Task SaveFlowScheduleList(ObjectId flowNodeId, List<FlowScheduleList> list)
        {
            var inserts = list
                .Where(l => l.State == FlowScheduleListState.Added)
                .ToList();
            if (inserts.Any())
            {
                foreach (var insert in inserts)
                {
                    var jobSchedule = insert.JobSchedule;
                    await JobSchedule
                        .InsertAsync(jobSchedule);

                    var node = new Node
                    {
                        Type = NodeType.FlowSchedule,
                        Name = "",
                        ParentId = flowNodeId,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    };
                    await Node
                        .InsertAsync(node);

                    var flowSchedule = new FlowSchedule
                    {
                        NodeId = node.NodeId,
                        JobScheduleId = jobSchedule.JobScheduleId
                    };
                    await FlowSchedule
                        .InsertAsync(flowSchedule);

                    _manualAgentWatcherNotification?.InvokeJobScheduleAddOrUpdated(jobSchedule);
                }

                await _nodeService.UpdateUtc(flowNodeId);
            }

            var updates = list
                .Where(l => l.State == FlowScheduleListState.Edited)
                .ToList();
            if (updates.Any())
            {
                foreach (var update in updates)
                {
                    var jobSchedule = update.JobSchedule;
                    await JobSchedule
                        .ReplaceAsync(js => js.JobScheduleId == jobSchedule.JobScheduleId, jobSchedule);

                    _manualAgentWatcherNotification?.InvokeJobScheduleAddOrUpdated(jobSchedule);
                }
            }

            var deletes = list
                .Where(l => l.State == FlowScheduleListState.Deleted)
                .ToList();
            if (deletes.Any())
            {
                foreach (var delete in deletes)
                {
                    var jobSchedule = delete.JobSchedule;

                    await DeleteTickerByJobScheduleId(jobSchedule.JobScheduleId);

                    await JobSchedule
                        .DeleteAsync(js => js.JobScheduleId == jobSchedule.JobScheduleId);

                    await FlowSchedule
                        .DeleteAsync(fs => fs.FlowScheduleId == delete.FlowSchedule.FlowScheduleId);

                    await Node
                        .DeleteAsync(n => n.NodeId == delete.FlowSchedule.NodeId);

                    _manualAgentWatcherNotification?.InvokeJobScheduleAddOrUpdated(null);
                }

                await _nodeService.UpdateUtc(flowNodeId);
            }
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
                        join js in JobSchedule.AsQueryable() on jt.JobScheduleId equals js.JobScheduleId
                        where now >= jt.TargetUtc
                        orderby jt.TargetUtc ascending
                        select new JobTickerAndSchedule
                        {
                            Ticker = jt,
                            Schedule = js
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

        public async Task CreateJobFromTicker(JobSchedule schedule)
        {
            if (schedule.ScheduleType == JobScheduleType.Single)
            {
                await DeactiveJobSchedule(schedule);
            }

            switch (schedule.JobType)
            {
                case JobType.CreateRun:
                    await CreateRunJob(schedule);
                    break;
                default:
                    await DeactiveJobSchedule(schedule);
                    break;
            }
        }

        private async Task CreateRunJob(JobSchedule schedule)
        {
            var job = new Job
            {
                Type = JobType.CreateRun,
                Status = JobStatus.Queued,
                Queued = DateTime.Now,

                FlowId = schedule.FlowId,
                RunInput = schedule.RunInput
            };

            await Job
                .InsertAsync(job);

            _manualAgentWatcherNotification?.InvokeJobQueued(job);
        }

        public Task DeactiveJobSchedule(JobSchedule schedule)
        {
            schedule.Active = false;

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
