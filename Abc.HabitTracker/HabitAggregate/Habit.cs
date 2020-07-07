using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Abc.HabitTracker
{
    public class RequestData
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("days_off")]
        public String[] DaysOff { get; set; }
    }
    public class Habit
    {
        //Entity
        private Guid _id;
        private string _name;
        private string[] _days_off;
        private Log _logs = new Log();
        private Guid _user_id;
        private DateTime _created_at;

        [JsonPropertyName("id")]
        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        [JsonPropertyName("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        [JsonPropertyName("days_off")]
        public string[] DaysOff
        {
            get
            {
                return _days_off;
            }
        }

        [JsonPropertyName("current_streak")]
        public int CurrentStreak
        {
            get
            {
                return _logs.current_streak;
            }
        }

        [JsonPropertyName("longest_streak")]
        public int LongestStreak
        {
            get
            {
                return _logs.longest_streak;
            }
        }

        [JsonPropertyName("log_count")]
        public int LogCount
        {
            get
            {
                return _logs.LogsCount;
            }
        }

        [JsonPropertyName("logs")]
        public DateTime[] Log
        {
            get
            {
                return _logs.Logs;
            }
        }

        [JsonPropertyName("user_id")]
        public Guid UserID
        {
            get
            {
                return _user_id;
            }
        }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt
        {
            get
            {
                return _created_at;
            }
        }

        public Habit(Guid id, string name, string[] days_off) : this(name, days_off)
        {
            this._id = id;
        }

        public Habit(Guid id, string name, string[] days_off, Guid user_id, DateTime created_at) : this(id, name, days_off)
        {
            this._user_id = user_id;
            this._created_at = created_at;
        }

        public Habit(Guid id, string name, string[] days_off, Guid user_id, Log logs) : this(id, name, days_off)
        {
            this._user_id = user_id;
            this._logs = logs;
        }

        public Habit() { }

        public static Habit NewHabit(string name, string[] days_off, Guid user_id)
        {
            string day = (DateTime.Today.DayOfWeek.ToString()).Substring(0, 3);

            return new Habit(Guid.NewGuid(), name, days_off, user_id, new Log(new DateTime[] { DateTime.Now }, new string[] { day }));
        }

        public Habit(string name, string[] days_off, Guid user_id) : this(name, days_off)
        {
            this._user_id = user_id;
        }

        public Habit(string name, string[] days_off)
        {
            if (days_off.Count() >= 7)
            {
                throw new Exception("Days off may not be as many as 7");
            }
            if (name.Length < 2 || name.Length > 100)
            {
                throw new Exception("name length must between 2 to 100");
            }
            if (DaysOffValidation(days_off) != true)
            {
                throw new Exception("Make sure the days off are same format and not duplicate");
            }

            this._id = Guid.NewGuid();
            this._name = name;
            this._created_at = DateTime.Now;
            this._days_off = days_off;
        }

        protected List<IObserver<Trigger>> _observers = new List<IObserver<Trigger>>();
        public void Attach(IObserver<Trigger> obs)
        {
            _observers.Add(obs);
        }

        public void Broadcast(Trigger e)
        {
            foreach (var obs in _observers)
            {
                obs.Update(e);
            }
        }

        public void AddLog(Log log)
        {
            this._logs = this._logs.Add(log);
        }

        private Boolean DaysOffValidation(string[] days_off)
        {
            string[] weekDays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            int size = days_off.Count();
            int countDupli = 0;
            int count = 0;

            for (int j = 0; j < weekDays.Count(); j++)
            {
                for (int i = 0; i < size; i++)
                {
                    if (weekDays[j].Equals(days_off[i])) countDupli++;
                    if (countDupli > 1) return false;
                }
                count += countDupli;
                countDupli = 0;
            }
            if (count != size) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            var habit = obj as Habit;
            if (habit == null) return false;

            return this._id == habit._id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}