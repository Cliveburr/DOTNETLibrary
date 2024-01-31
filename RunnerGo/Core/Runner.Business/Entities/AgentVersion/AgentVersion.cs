using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.AgentVersion
{
    [DatabaseDef]
    public class AgentVersion
    {
        [BsonId]
        public ObjectId AgentVersionId { get; set; }
        public DateTime CreatedUtc { get; set; }
        [IndexDef]
        public int Version { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileContent { get; set; }
    }
}
