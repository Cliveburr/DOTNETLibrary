using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    public class Agent
    {
        [BsonId]
        public ObjectId AgentId { get; set; }
        public required ObjectId NodeId { get; set; }
        public required string MachineName { get; set; }
        public required List<string> RegistredTags { get; set; }
        public List<string>? ExtraTags { get; set; }
        public DateTime LastExecuted { get; set; }
        public AgentStatus Status { get; set; }
        public bool Enabled { get; set; }
    }
}
