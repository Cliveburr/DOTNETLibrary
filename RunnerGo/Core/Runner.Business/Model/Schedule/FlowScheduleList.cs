using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Model.Schedule
{
    public enum FlowScheduleListState
    {
        Pristine = 0,
        Added = 1,
        Edited = 2,
        Deleted = 3
    }

    public class FlowScheduleList
    {
        public FlowScheduleListState State { get; set; }
        public required FlowSchedule FlowSchedule { get; set; }
        public required JobSchedule JobSchedule { get; set; }

        public string Display()
        {
            var disabled = JobSchedule.Active ? "" : " <disabled>";
            return JobSchedule.ScheduleType switch
            {
                JobScheduleType.Single => $"Single run at {JobSchedule.SingleDateTimeUtc?.ToLocalTime()}{disabled}",
                JobScheduleType.Interval => $"Interval of {JobSchedule.IntervalSecond} seconds{disabled}",
                JobScheduleType.Daily => $"{FormatDaysOfWeek(JobSchedule.DailyTime, JobSchedule.DaysOfWeek)}{disabled}",
                _ => $"Invalid JobScheduleType"
            };
        }

        private string FormatDaysOfWeek(TimeSpan? dailyTime, string? daysOfWeek)
        {
            var dailyTimeStr = (dailyTime ?? TimeSpan.MinValue)
                .ToString(@"hh\:mm\:ss");

            var days = new List<string>();
            if (daysOfWeek is not null && daysOfWeek.Length == 7)
            {
                for (var i = 0; i < 7; i++)
                {
                    if (daysOfWeek[i] != '1')
                    {
                        continue;
                    }

                    var dayOfWeek = (DayOfWeek)i;
                    days.Add(dayOfWeek.ToString());
                }
            }
            var daysOfWeekStr = string.Join(", ", days);

            return $"Daily on {dailyTimeStr} of days {daysOfWeekStr}";
        }
    }
}
