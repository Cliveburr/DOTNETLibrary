using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes.Types;
using Runner.Domain.Entities.Nodes;
using Runner.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Infrastructure.Collections
{
    internal class AppCollection : CollectionBase<App>
    {
        public override string Name => "App";

        public AppCollection(MainDatabase database)
            : base(database)
        {
        }

        public override FilterDefinition<App> CreateEntityIdFilter(App doc)
        {
            return Builders<App>.Filter
                .Eq(e => e.AppId, doc.AppId);
        }

        public void Get()
        {
            _collection.Aggregate()
        }
    }
}
