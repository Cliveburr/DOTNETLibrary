using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Business.Security;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Identity;
using Runner.Business.Model.Nodes.Types;
using System.Xml.Linq;

namespace Runner.Business.Services.NodeTypes
{
    public class AppService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public AppService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<List<Node>> ReadLoggedAppsAsNode()
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var query = from a in App.AsQueryable()
                        join n in Node.Collection on a.NodeId equals n.NodeId
                        where a.OwnerId == _identityProvider.User.UserId
                        select n;

            return query
                .ToListAsync();
        }

        public Task<AppView?> ReadView(string? identify)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            if (ObjectId.TryParse(identify, out var nodeId))
            {
                var query = from n in Node.AsQueryable()
                            join a in App.AsQueryable() on n.NodeId equals a.NodeId
                            where n.ParentId == null && n.NodeId == nodeId
                            select new AppView
                            {
                                App = a,
                                Node = n
                            };

                return query
                    .FirstOrDefaultAsync();

            }
            else
            {
                if (string.IsNullOrEmpty(identify))
                {
                    return Task.FromResult<AppView?>(null);
                }
                else
                {
                    var query = from a in App.AsQueryable()
                                join n in Node.Collection on a.NodeId equals n.NodeId
                                where n.ParentId == null && n.Name.ToLower().Equals(identify.ToLower())
                                select new AppView
                                {
                                    App = a,
                                    Node = n
                                };

                    return query
                        .FirstOrDefaultAsync();
                }
            }
        }

        public Task<Tuple<App, Node>?> ReadByName(string name)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var query = from a in App.AsQueryable()
                        join n in Node.Collection on a.NodeId equals n.NodeId into r
                        where r.FirstOrDefault() != null && r.First().Name.ToLower().Equals(name.ToLower())
                        select Tuple.Create(a, r.First());

            return query
                .FirstOrDefaultAsync();
        }

        public async Task Create(string? name)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, null);
            Assert.MustNull(has, "App already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.App,
                Name = name,
                ParentId = null,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            var app = new App
            {
                NodeId = node.NodeId,
                OwnerId = _identityProvider.User.UserId
            };
            await App
                .InsertAsync(app);
        }

        public async Task DeleteByNodeId(ObjectId nodeId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var childs = await _nodeService.HasChilds(nodeId);
            Assert.MustFalse(childs, "Node is note empty!");

            await App
                .DeleteAsync(a => a.NodeId == nodeId);

            await Node
                .DeleteAsync(n => n.NodeId == nodeId);
        }
    }
}
