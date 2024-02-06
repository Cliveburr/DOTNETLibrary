using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Data
    {
        [BsonId]
        public ObjectId DataId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public string? DataTypePath { get; set; }
        public ObjectId? DataTypeId { get; set; }
        public required List<DataProperty> Properties { get; set; }
    }
}
