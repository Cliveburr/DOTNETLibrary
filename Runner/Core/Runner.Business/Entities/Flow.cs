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
    }

    public class FlowActionContainer
    {
        public required string Label { get; set; }
        public List<FlowAction>? Actions { get; set; }
        public List<FlowActionContainer>? Next { get; set; }
    }
}
