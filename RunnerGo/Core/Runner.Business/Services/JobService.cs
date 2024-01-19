using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Job;
using Runner.Business.Security;

namespace Runner.Business.Services
{
    public class JobService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public JobService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<List<Job>> ReadForAgent(ObjectId agentId)
        {
            return Job
                .ToListAsync(j => j.AgentId == agentId);
        }
    }
}
