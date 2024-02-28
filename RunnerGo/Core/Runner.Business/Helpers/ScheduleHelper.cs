using System.Globalization;

namespace Runner.Business.Helpers
{
    public static class ScheduleHelper
    {
        //public static DateTime? ComputNextTicker(TimeSpan dailyTime, List<string> dailyDayNames, DateTime from)
        //{
        //    var todayPassed = from.TimeOfDay <= dailyTime ? 0 : 1;

        //    for (var i = 0; i < 7; i++)
        //    {
        //        var day = from.Date.AddDays(i + todayPassed);
        //        var dayName = day.ToString("ddd", new CultureInfo("en-US"));

        //        if (dailyDayNames.Contains(dayName))
        //        {
        //            return day.Add(dailyTime);
        //        }
        //    }

        //    return null;
        //}

        public static DateTime? ComputNextTicker(TimeSpan dailyTime, string daysOfWeek, DateTime fromUtc)
        {
            Assert.MustTrue(daysOfWeek.Length == 7, "DaysOfWeek invalid format!");

            var dayNames = new List<DayOfWeek>();
            for (var i = 0; i < 7; i++)
            {
                if (daysOfWeek[i] == '1')
                {
                    dayNames.Add((DayOfWeek)i);
                }
            }

            var from = fromUtc.ToLocalTime();
            var todayPassed = from.TimeOfDay <= dailyTime ? 0 : 1;

            for (var i = 0; i < 7; i++)
            {
                var day = from.Date.AddDays(i + todayPassed);
                if (dayNames.Contains(day.DayOfWeek))
                {
                    return day.Add(dailyTime)
                        .ToUniversalTime();
                }
            }

            return null;
        }
    }
}
