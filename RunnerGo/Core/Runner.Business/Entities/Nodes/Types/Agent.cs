using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Agent
    {
        [BsonId]
        public ObjectId AgentId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required string MachineName { get; set; }
        public required string VersionName { get; set; }
        public required List<string> RegistredTags { get; set; }
        public List<string>? ExtraTags { get; set; }
        public DateTime LastExecuted { get; set; }
        public AgentStatus Status { get; set; }
        public bool Enabled { get; set; }
    }
}
