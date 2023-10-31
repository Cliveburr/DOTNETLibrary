using MongoDB.Bson.Serialization.Attributes;
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
        public required FlowActionContainer Root { get; set; }
    }

    public class FlowAction
    {
        public required string Label { get; set; }
        public string? AgentPool { get; set; }
        public List<string>? Tags { get; set; }
    }

    public class FlowActionContainer
    {
        public required string Label { get; set; }
        public string? AgentPool { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsForActions { get; set; }
        public List<FlowAction>? ContentActions { get; set; }
        public List<FlowActionContainer>? ContentContainers { get; set; }
        public List<FlowActionContainer>? Next { get; set; }
    }
}
