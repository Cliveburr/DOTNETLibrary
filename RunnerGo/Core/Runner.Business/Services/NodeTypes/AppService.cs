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

namespace Runner.Business.Services.NodeTypes
{
    public class AppService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;

        public AppService(Database database, IdentityProvider identityProvider)
            : base(database)
        {
            _identityProvider = identityProvider;
        }

        public Task<List<App>> ReadLoggedApps()
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var query = from a in App.AsQueryable()
                        select a;

            return query
                .ToListAsync();

        //    FieldDefinition<Node, ObjectId> field = "NodeId";


                       //    var query = from a in App.AsQueryable()
                       //                join n in Node.Collection on a.NodeId equals n.NodeId into r
                       //                select r;

                       //var test = App.Aggregate()
                       //        .Match(a => a.OwnerId == _identityProvider.User.UserId)
                       //        .Lookup(Node.Name, a => a.NodeId, field, "")
                       //        .ToList();

                       //    var project = Builders<Node>.Projection.Expression(n => new AppList
                       //    {
                       //        Name = n.Name,
                       //        Type = n.Type,
                       //    });


                       //    return Node
                       //        .ProjectToListAsync(n => )
                       //        .ToListAsync<App>(a => a.Parent == null && a.Type == NodeType.App && a.OwnerId == _userLogged.User.Id);
        }
    }
}
