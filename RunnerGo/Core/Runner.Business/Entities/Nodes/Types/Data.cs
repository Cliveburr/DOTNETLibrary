using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas2.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Data
    {
        [BsonId]
        public ObjectId DataId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required List<DataProperty> Properties { get; set; }
    }
}
