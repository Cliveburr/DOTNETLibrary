using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class ScriptContent
    {
        [BsonId]
        public ObjectId ScriptContentId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileContent { get; set; }
    }
}
