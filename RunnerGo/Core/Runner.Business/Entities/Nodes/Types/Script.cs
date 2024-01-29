using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Runner.Business.Entities.Nodes.Types
{
    public class Script
    {
        [BsonId]
        public ObjectId ScriptId { get; set; }
        public required ObjectId NodeId { get; set; }
        public required Dictionary<int, ScriptVersion> Versions { get; set; }
    }

    public class ScriptVersion
    {
        public int Version { get; set; }
        public ObjectId ScriptContentId { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required List<DataTypeProperty> InputTypes { get; set; }
        public required List<DataTypeProperty> OutputTypes { get; set; }
    }
}
