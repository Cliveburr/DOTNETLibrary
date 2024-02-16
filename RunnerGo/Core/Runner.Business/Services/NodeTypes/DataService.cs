using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Datas.Model;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class DataService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public DataService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<Data?> ReadByNodeId(ObjectId nodeId)
        {
            return Data
                .FirstOrDefaultAsync(dt => dt.NodeId == nodeId);
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "Data name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.Data,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            await _nodeService.UpdateUtc(parentId);

            var data = new Data
            {
                NodeId = node.NodeId,
                Properties = new List<DataProperty>()
            };
            await Data
                .InsertAsync(data);
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await Data
                .DeleteAsync(d => d.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }

        public async Task Update(Data data)
        {
            await _nodeService.UpdateUtc(data.NodeId);

            await Data
                .ReplaceAsync(dt => dt.DataId == data.DataId, data);
        }
    }
}
