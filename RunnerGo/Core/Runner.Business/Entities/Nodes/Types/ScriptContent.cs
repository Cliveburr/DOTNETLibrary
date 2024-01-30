using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Runner.Business.Entities.Nodes.Types
{
    public class ScriptContent
    {
        [BsonId]
        public ObjectId ScriptContentId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileContent { get; set; }
    }
}
