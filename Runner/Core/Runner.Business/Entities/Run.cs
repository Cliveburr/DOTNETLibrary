using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities
{
    [BsonDiscriminator("Run")]
    public class Run : NodeBase
    {
        public required List<Actions.Action> Actions { get; set; }
        public required List<ActionContainer> Containers { get; set; }
        public int RootContainerId { get; set; }
        public int IdIndexes { get; set; }
        public required List<RunLog> Log { get; set; }
    }

    public class RunLog
    {
        public DateTime Created { get; set; }
        public required string Text { get; set; }
    }
}
