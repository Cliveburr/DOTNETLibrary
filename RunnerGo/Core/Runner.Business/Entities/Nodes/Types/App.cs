using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class App
    {
        [BsonId]
        public ObjectId AppId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        [IndexDef]
        public required ObjectId OwnerId { get; set; }
    }
}
