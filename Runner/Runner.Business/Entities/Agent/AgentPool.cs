using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Agent
{
    [BsonDiscriminator("AgentPool")]
    public class AgentPool : NodeBase
    {
        public required List<Agent> Agents { get; set; }
    }
}
