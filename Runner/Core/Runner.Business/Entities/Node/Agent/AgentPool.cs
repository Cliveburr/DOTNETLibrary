using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node.Agent
{
    [BsonDiscriminator("AgentPool")]
    public class AgentPool : Node
    {
        public bool Enabled { get; set; }
    }
}
