using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Runner.Business.Entities.Nodes.Types
{
    public class Folder
    {
        [BsonId]
        public ObjectId FolderId { get; set; }
        public required ObjectId NodeId { get; set; }
    }
}
