using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.DataAccess.Entities
{
    public class Configurations
    {
        [BsonId]
        public required string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
