using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class ScriptPackage
    {
        [BsonId]
        public ObjectId ScriptPackageId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public ObjectId? ExtractJobId { get; set; }
        public string? LastWarnings { get; set; }
    }
}
