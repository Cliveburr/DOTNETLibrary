using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Datas2.Model
{
    public class DataValue
    {
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public List<string>? StringListValue { get; set; }
        public ObjectId? ObjectIdValue { get; set; }

        [BsonIgnore]
        public string? NodePath { get; set; }
        [BsonIgnore]
        public List<DataHandlerItem>? DataExpand { get; set; }

        public DataValue Clone()
        {
            return new DataValue
            {
                StringValue = StringValue,
                StringListValue = StringListValue,
                ObjectIdValue = ObjectIdValue
            };
        }
    }
}
