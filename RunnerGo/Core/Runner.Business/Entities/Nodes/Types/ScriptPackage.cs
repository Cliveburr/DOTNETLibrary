using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Runner.Business.Entities.Nodes.Types
{
    public class ScriptPackage
    {
        [BsonId]
        public ObjectId ScriptPackageId { get; set; }
        public required ObjectId NodeId { get; set; }
        public ObjectId? ExtractJobId { get; set; }
    }
}
