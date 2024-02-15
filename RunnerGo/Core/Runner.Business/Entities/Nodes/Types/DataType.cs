using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas2.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class DataType
    {
        [BsonId]
        public ObjectId DataTypeId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required DataObject Object { get; set; }
    }
}
