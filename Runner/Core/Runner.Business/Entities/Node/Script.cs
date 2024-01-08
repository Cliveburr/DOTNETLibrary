using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    [BsonDiscriminator("Script")]
    public class Script : NodeBase
    {
        public required Dictionary<int, ScriptVersion> Versions { get; set; }
    }

    public class ScriptVersion
    {
        public int Version { get; set; }
        public ObjectId FileStorageId { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required DataTypeStruct Input { get; set; }
        public required DataTypeStruct Output { get; set; }
    }
}
