using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Domain.Entities.Nodes;
using Runner.Infrastructure.DataAccess;

namespace Runner.Infrastructure.Collections
{
    internal class NodeCollection : CollectionBase<Node>
    {
        public override string Name => "Node";

        public NodeCollection(MainDatabase database)
            : base(database)
        {
        }

        public override FilterDefinition<Node> CreateEntityIdFilter(Node doc)
        {
            return Builders<Node>.Filter
                .Eq(u => u.NodeId, doc.NodeId);
        }
    }
}
