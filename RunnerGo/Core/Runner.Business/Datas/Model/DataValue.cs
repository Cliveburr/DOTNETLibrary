using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Datas.Model
{
    public class DataValue
    {
        public string? StringKind { get; set; }
        public List<string>? ListStringKind { get; set;}
        public ObjectId? ObjectIdKind { get; set; }

        [BsonIgnore]
        public List<DataProperty>? ObjectExpand { get; set; }
    }
}
