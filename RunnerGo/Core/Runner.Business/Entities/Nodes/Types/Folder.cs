using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Folder
    {
        [BsonId]
        public ObjectId FolderId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
    }
}
