using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    [BsonDiscriminator("ScriptPackage")]
    public class ScriptPackage : Node
    {
        public int NextVersion { get; set; }
        public ObjectId? TaskScheduleId { get; set; }
    }
}
