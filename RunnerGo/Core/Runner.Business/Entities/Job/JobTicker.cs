using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Job
{
    [DatabaseDef]
    public class JobTicker
    {
        [BsonId]
        public ObjectId JobTickerId { get; set; }
        [IndexDef]
        public DateTime TargetUtc { get; set; }
        public required ObjectId JobScheduleId { get; set; }
    }
}
