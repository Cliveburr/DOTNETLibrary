using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class AgentPoolService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public AgentPoolService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<List<Agent>> ReadAgentForNodeId(ObjectId nodeId)
        {
            var query = from a in Agent.AsQueryable()
                        join n in Node.AsQueryable() on a.NodeId equals n.ParentId
                        select a;

            return query
                .ToListAsync();
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "AgentPool name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.AgentPool,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            await _nodeService.UpdateUtc(parentId);

            var agentPool = new AgentPool
            {
                NodeId = node.NodeId
            };
            await AgentPool
                .InsertAsync(agentPool);
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var childs = await _nodeService.HasChilds(node.NodeId);
            Assert.MustFalse(childs, "Node is note empty!");

            await AgentPool
                .DeleteAsync(ap => ap.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }
    }
}
