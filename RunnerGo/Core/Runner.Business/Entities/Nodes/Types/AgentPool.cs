using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class AgentPool
    {
        [BsonId]
        public ObjectId AgentPoolId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public bool Enabled { get; set; }
    }
}
