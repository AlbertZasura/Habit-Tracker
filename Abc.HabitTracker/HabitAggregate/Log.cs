using System;

using System.Linq;
namespace Abc.HabitTracker
{
    public class Log
    {
        //ValueObject
        private DateTime[] _logs = { };
        private string[] _days = { };
        private int _current_streak;
        private int _longest_streak;

        public Log(DateTime[] logs, string[] days) : this(1, 1, logs, days) { }

        public Log() { }

        public Log(int current_streak, int longest_streak, DateTime[] logs, string[] days)
        {
            this._days = days;
            this._current_streak = current_streak;
            this._longest_streak = longest_streak;
            this._logs = logs;
        }

        public DateTime[] Logs
        {
            get
            {
                return _logs;
            }
        }

        public string[] Days
        {
            get
            {
                return _days;
            }
        }

        public int LogsCount
        {
            get
            {
                return _logs.Count();
            }
        }

        public int current_streak
        {
            get
            {
                return _current_streak;
            }
        }
        public int longest_streak
        {
            get
            {
                return _longest_streak;
            }
        }

        public static Log newLog()
        {
            string[] currentDay = { (DateTime.Today.DayOfWeek.ToString()).Substring(0, 3) };
            DateTime[] currentDate = { DateTime.Now};
            return new Log(currentDate,currentDay);
        }

        public Log Add(Log log)
        {
            if (log._longest_streak == 0)
            {
                return new Log();
            }
            return new Log(log._current_streak, log._longest_streak, log._logs, log._days);
        }

        public override bool Equals(object obj)
        {
            var m = obj as Log;
            if (m == null) return false;

            if (this._current_streak != m._current_streak) return false;

            if (this._longest_streak != m._longest_streak) return false;

            if (this._days != m._days) return false;

            if (this._logs != m._logs) return false;

            return true;
        }

        public Log AddCurrentStreak()
        {
            return new Log(_current_streak + 1, _longest_streak, _logs, _days);

        }

        public Log AddLongestStreak()
        {
            return new Log(_current_streak, _current_streak, _logs, _days);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}