using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes
{
    [DatabaseDef]
    public class Node
    {
        [BsonId]
        public ObjectId NodeId { get; set; }
        public NodeType Type { get; set; }
        [IndexDef]
        public required string Name { get; set; }
        [IndexDef]
        public ObjectId? ParentId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
    }
}
