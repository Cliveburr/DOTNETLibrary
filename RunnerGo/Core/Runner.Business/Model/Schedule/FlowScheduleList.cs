using MongoDB.Bson;
using Runner.Business.Entities.Job;

namespace Runner.Business.Model.Schedule
{
    public class FlowScheduleList
    {
        public required ObjectId FlowScheduleId { get; set; }
        public required JobSchedule JobSchedule { get; set; }

        public string Display()
        {
            return JobSchedule.ScheduleType switch
            {
                JobScheduleType.Single => $"Single run at {JobSchedule.SingleDateTimeUtc}",
                JobScheduleType.Interval => $"Interval of {JobSchedule.IntervalSecond} seconds",
                JobScheduleType.Daily => $"Daily on {JobSchedule.DailyTime} of days {string.Join(", ", JobSchedule.DailyDayNames ?? [])}",
                _ => $"Invalid JobScheduleType"
            };
        }
    }
}
