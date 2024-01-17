using Runner.Business.DataAccess;
using Runner.Business.Security;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Runner.Business.Model.Nodes.Types;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Services.NodeTypes
{
    public class FolderService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public FolderService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<FolderView?> ReadView(string? identify)
        {
            if (ObjectId.TryParse(identify, out var nodeId))
            {
                var query = from n in Node.AsQueryable()
                            join f in Folder.AsQueryable() on n.NodeId equals f.NodeId
                            where n.NodeId == nodeId
                            select new FolderView
                            {
                                Folder = f,
                                Node = n
                            };

                return query
                    .FirstOrDefaultAsync();
            }
            else
            {
                if (string.IsNullOrEmpty(identify))
                {
                    return Task.FromResult<FolderView?>(null);
                }
                else
                {
                    var query = from n in Node.AsQueryable()
                                join f in Folder.AsQueryable() on n.NodeId equals f.NodeId
                                where n.ParentId == null && n.Name.ToLower().Equals(identify.ToLower())
                                select new FolderView
                                {
                                    Folder = f,
                                    Node = n
                                };

                    return query
                        .FirstOrDefaultAsync();
                }
            }
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "Folder name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.Folder,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            var folder = new Folder
            {
                NodeId = node.NodeId
            };
            await Folder
                .InsertAsync(folder);
        }

        public async Task DeleteByNodeId(ObjectId nodeId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var childs = await _nodeService.HasChilds(nodeId);
            Assert.MustFalse(childs, "Node is note empty!");

            await Folder
                .DeleteAsync(a => a.NodeId == nodeId);

            await Node
                .DeleteAsync(n => n.NodeId == nodeId);
        }
    }
}
