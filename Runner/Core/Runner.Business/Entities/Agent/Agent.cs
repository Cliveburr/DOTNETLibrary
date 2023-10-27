using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Agent
{
    [BsonDiscriminator("Agent")]
    public class Agent : NodeBase
    {
        public required string MachineName { get; set; }
        public required List<string> RegistredTags { get; set; }
        public List<string>? ExtraTags { get; set; }
        public DateTime HeartBeat { get; set; }
        public DateTime LastExecuted { get; set; }
        public AgentStatus Status { get; set; }
        public bool Enabled { get; set; }
    }
}
