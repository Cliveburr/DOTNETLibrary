using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Script
    {
        [BsonId]
        public ObjectId ScriptId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public int NextVersion { get; set; }
        public required List<ScriptVersion> Versions { get; set; }
    }

    public class ScriptVersion
    {
        public int Version { get; set; }
        public ObjectId ScriptContentId { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required List<DataProperty> Input { get; set; }
    }
}
