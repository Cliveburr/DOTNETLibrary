using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes;
using Runner.Business.Model.Nodes.Types;
using MongoDB.Driver;
using Runner.Business.Entities.Node;

namespace Runner.Business.Services.Nodes
{
    public class AppService : ServiceBase
    {
        private readonly UserLogged _userLogged;
        private readonly NodeService _nodeService;

        public AppService(Database database, UserLogged userLogged, NodeService nodeService)
            : base(database)
        {
            _userLogged = userLogged;
            _nodeService = nodeService;
        }

        public Task<List<AppList>> ReadLoggedApps()
        {
            Assert.MustNotNull(_userLogged.User, "Not logged!");

            App._collection.Aggregate()
                .Match(a => a.OwnerId == _userLogged.User.UserId)
                .Lookup<>

            var project = Builders<Node>.Projection.Expression(n => new AppList
            {
                Name = n.Name,
                Type = n.Type,
            });


            return Node
                .ProjectToListAsync(n => )
                .ToListAsync<App>(a => a.Parent == null && a.Type == NodeType.App && a.OwnerId == _userLogged.User.Id);
        }

        public async Task Create(string name)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            var node = new Node
            {
                Name = name,
                Type = NodeType.App,
                ParentId = null
            };
            await Node.CreateAsync(node);

            var app = new App
            {
                NodeId = node.NodeId,
                OwnerId = _userLogged.User.UserId
            };
            await App.CreateAsync(app);
        }
    }
}
