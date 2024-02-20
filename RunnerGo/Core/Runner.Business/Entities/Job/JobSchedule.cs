using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Job
{
    [DatabaseDef]
    public class JobSchedule
    {
        [BsonId]
        public ObjectId JobScheduleId { get; set; }
        public required bool Active { get; set; }
        public required JobScheduleType ScheduleType { get; set; }
        public JobType JobType { get; set; }

        public DateTime? SingleDateTimeUtc { get; set; }
        public int? IntervalSecond { get; set; }
        public TimeSpan? DailyTime { get; set; }
        public List<string>? DailyDayNames { get; set; }
    }
}
