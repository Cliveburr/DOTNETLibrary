using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    [BsonDiscriminator("Flow")]
    public class Flow : Node
    {
        public required FlowAction Root { get; set; }
    }

    public class FlowAction
    {
        public required string Label { get; set; }
        public string? AgentPool { get; set; }
        public List<string>? Tags { get; set; }
        public ActionType Type { get; set; }
        public List<FlowAction>? Childs { get; set; }
    }
}
