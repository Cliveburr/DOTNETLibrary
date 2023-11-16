using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.ActionsOutro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities
{
    [BsonDiscriminator("Flow")]
    public class Flow : NodeBase
    {
        public required FlowAction2 Root { get; set; }
    }

    public class FlowAction2
    {
        public required string Label { get; set; }
        public string? AgentPool { get; set; }
        public List<string>? Tags { get; set; }
        public ActionType Type { get; set; }
        public List<FlowAction2>? Childs { get; set; }
    }
}
