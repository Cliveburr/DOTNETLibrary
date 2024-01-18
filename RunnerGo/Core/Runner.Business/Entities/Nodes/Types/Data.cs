using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    public class Data
    {
        [BsonId]
        public ObjectId DataId { get; set; }
        public required ObjectId NodeId { get; set; }
        public string? DataTypePath { get; set; }
        public ObjectId? DataTypeId { get; set; }
        public required List<DataProperty> Properties { get; set; }
    }

    public class DataProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
