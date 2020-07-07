using System;
using Xunit;

namespace Abc.HabitTracker.Test
{
    public class HabitTest
    {
        string name = "sleep";
        Habit h;
        [Fact]
        public void DuplicateDaysOff()
        {
            string[] days_off = { "Sun", "Sun", "Tue", "Wed", "Thu", "Fri" };

            Exception ex = Assert.Throws<Exception>(() => h = new Habit(name, days_off));
            Assert.Equal("Make sure the days off are same format and not duplicate", ex.Message);
        }

        [Fact]
        public void NotDaysOff()
        {
            string[] days_off = { "Sun", "Sat", "tue", "Wed", "Thu", "Fri" };
            Exception ex = Assert.Throws<Exception>(() => h = new Habit(name, days_off));
            Assert.Equal("Make sure the days off are same format and not duplicate", ex.Message);
        }
        [Fact]
        public void Holiday()
        {
            string[] days_off = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            Exception ex = Assert.Throws<Exception>(() => h = new Habit(name, days_off));
            Assert.Equal("Days off may not be as many as 7", ex.Message);
        }
        [Fact]
        public void HabitLength()
        {
            string[] days_off = { "Sun", "Tue", "Wed", "Thu", "Fri", "Sat" };
            Exception ex = Assert.Throws<Exception>(() => h = new Habit("a", days_off));
            Assert.Equal("name length must between 2 to 100", ex.Message);
        }
    }
}
