using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    public class AgentPool
    {
        [BsonId]
        public ObjectId AgentPoolId { get; set; }
        public required ObjectId NodeId { get; set; }
        public bool Enabled { get; set; }
    }
}
