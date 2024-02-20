using Runner.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Helpers
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void NextOnSameDay()
        {
            var from = DateTime.Parse("10:00 20/02/2024");  //Tue
            var dailyTime = TimeSpan.Parse("12:00");
            var dailyDayNames = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            var expected = DateTime.Parse("12:00 20/02/2024");  //Tue

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, dailyDayNames, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnNextDay()
        {
            var from = DateTime.Parse("12:00 20/02/2024");  //Tue
            var dailyTime = TimeSpan.Parse("01:00");
            var dailyDayNames = new List<string> { "Sun", "Mon", "Wed", "Thu", "Fri", "Sat" };
            var expected = DateTime.Parse("01:00 21/02/2024");  //Wed

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, dailyDayNames, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnDayBehind()
        {
            var from = DateTime.Parse("10:00 20/02/2024");  //Tue
            var dailyTime = TimeSpan.Parse("12:00");
            var dailyDayNames = new List<string> { "Mon" };
            var expected = DateTime.Parse("12:00 26/02/2024");  //Wed

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, dailyDayNames, from);

            Test.AreEqual(expected, computed);
        }
    }
}
