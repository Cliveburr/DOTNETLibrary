using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class DataTypeService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public DataTypeService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<DataType?> ReadById(ObjectId dataTypeId)
        {
            return DataType
                .FirstOrDefaultAsync(dt => dt.DataTypeId == dataTypeId);
        }

        public Task<DataType?> ReadByNodeId(ObjectId nodeId)
        {
            return DataType
                .FirstOrDefaultAsync(dt => dt.NodeId == nodeId);
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "DataType name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.DataType,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            await _nodeService.UpdateUtc(parentId);

            var dataType = new DataType
            {
                NodeId = node.NodeId,
                Properties = new List<DataTypeProperty>()
            };
            await DataType
                .InsertAsync(dataType);
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await DataType
                .DeleteAsync(a => a.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }

        public async Task Update(DataType dataType)
        {
            await _nodeService.UpdateUtc(dataType.NodeId);

            await DataType
                .ReplaceAsync(dt => dt.DataTypeId == dataType.DataTypeId, dataType);
        }
    }
}
