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
            //var dailyDayNames = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            var daysOfWeek = "1111111";
            var expected = DateTime.Parse("12:00 20/02/2024");  //Tue

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, daysOfWeek, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnNextDay()
        {
            var from = DateTime.Parse("12:00 20/02/2024");  //Tue
            var dailyTime = TimeSpan.Parse("01:00");
            //var dailyDayNames = new List<string> { "Sun", "Mon", "Wed", "Thu", "Fri", "Sat" };
            // Sun, Mon, Tue, Wed, Thu, Fri, Sat
            var daysOfWeek = "1101111";
            var expected = DateTime.Parse("01:00 21/02/2024");  //Wed

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, daysOfWeek, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnDayBehind()
        {
            var from = DateTime.Parse("10:00 20/02/2024");  //Tue
            var dailyTime = TimeSpan.Parse("12:00");
            //var dailyDayNames = new List<string> { "Mon" };
            // Sun, Mon, Tue, Wed, Thu, Fri, Sat
            var daysOfWeek = "0100000";
            var expected = DateTime.Parse("12:00 26/02/2024");  //Wed

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, daysOfWeek, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnMSundayBehind()
        {
            var from = DateTime.Parse("12:01 24/02/2024");  //Sat
            var dailyTime = TimeSpan.Parse("12:00");
            //var dailyDayNames = new List<string> { "Mon" };
            // Sun, Mon, Tue, Wed, Thu, Fri, Sat
            var daysOfWeek = "1000001";
            var expected = DateTime.Parse("12:00 25/02/2024");  //Sun

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, daysOfWeek, from);

            Test.AreEqual(expected, computed);
        }

        [TestMethod]
        public void NextOnMSatBehind()
        {
            var from = DateTime.Parse("12:01 24/02/2024");  //Sat
            var dailyTime = TimeSpan.Parse("12:00");
            //var dailyDayNames = new List<string> { "Mon" };
            // Sun, Mon, Tue, Wed, Thu, Fri, Sat
            var daysOfWeek = "0000001";
            var expected = DateTime.Parse("12:00 02/03/2024");  //Sun

            var computed = ScheduleHelper.ComputNextTicker(dailyTime, daysOfWeek, from);

            Test.AreEqual(expected, computed);
        }
    }
}
