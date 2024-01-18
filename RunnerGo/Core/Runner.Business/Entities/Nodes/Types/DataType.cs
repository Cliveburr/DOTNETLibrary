using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    public class DataType
    {
        [BsonId]
        public ObjectId DataTypeId { get; set; }
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
