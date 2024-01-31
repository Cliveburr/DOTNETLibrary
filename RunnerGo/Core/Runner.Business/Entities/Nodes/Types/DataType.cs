using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class DataType
    {
        [BsonId]
        public ObjectId DataTypeId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required List<DataTypeProperty> Properties { get; set; }
    }

    public class DataTypeProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Default { get; set; }
        public bool IsRequired { get; set; }
    }
}
