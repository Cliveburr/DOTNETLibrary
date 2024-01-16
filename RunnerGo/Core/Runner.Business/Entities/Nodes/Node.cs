using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes
{
    public class Node
    {
        [BsonId]
        public ObjectId NodeId { get; set; }
        public NodeType Type { get; set; }
        public ObjectId? ParentId { get; set; }
    }
}
