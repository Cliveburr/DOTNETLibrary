using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    public class App
    {
        [BsonId]
        public ObjectId AppId { get; set; }
        public required ObjectId NodeId { get; set; }
        public required string Name { get; set; }
        public required ObjectId OwnerId { get; set; }
    }
}
