using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Runner.Business.Entities.AgentVersion
{
    public class AgentVersion
    {
        [BsonId]
        public ObjectId AgentVersionId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public int Version { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileContent { get; set; }
    }
}
