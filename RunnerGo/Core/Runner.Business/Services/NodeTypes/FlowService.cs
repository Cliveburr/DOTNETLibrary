using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class FlowService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public FlowService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public async Task<Flow> ReadByNodeId(ObjectId nodeId)
        {
            var flow = await Flow
                .FirstOrDefaultAsync(f => f.NodeId == nodeId);
            Assert.MustNotNull(flow, "Flow not found for NodeId: " + nodeId);
            return flow;
        }

        public async Task Update(Flow flow)
        {
            await _nodeService.UpdateUtc(flow.FlowId);

            await Flow
                .ReplaceAsync(f => f.FlowId == flow.FlowId, flow);
        }

        public async Task Delete(Flow flow)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var hasRuns = await Run
                .AnyAsync(n => n.FlowId == flow.FlowId);
            Assert.MustFalse(hasRuns, "Need to clean all runs of this Flow to delete!");

            var node = await _nodeService.ReadByNodeId(flow.NodeId);
            Assert.MustNotNull(node, "Internal - Node for the Flow not found!");

            await Flow
                .DeleteAsync(a => a.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "Node name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.Flow,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            await _nodeService.UpdateUtc(parentId);

            var flow = new Flow
            {
                NodeId = node.NodeId,
                Root = new FlowAction
                {
                    Label = "Root",
                    Type = Actions.ActionType.Script,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "Action"
                        }
                    }
                }
            };
            await Flow
                .InsertAsync(flow);
        }
    }
}
