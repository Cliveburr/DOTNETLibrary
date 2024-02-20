using System.Globalization;

namespace Runner.Business.Helpers
{
    public static class ScheduleHelper
    {
        public static DateTime? ComputNextTicker(TimeSpan dailyTime, List<string> dailyDayNames, DateTime from)
        {
            var todayPassed = from.TimeOfDay <= dailyTime ? 0 : 1;

            for (var i = 0; i < 7; i++)
            {
                var day = from.Date.AddDays(i + todayPassed);
                var dayName = day.ToString("ddd", new CultureInfo("en-US"));

                if (dailyDayNames.Contains(dayName))
                {
                    return day.Add(dailyTime);
                }
            }

            return null;
        }
    }
}
