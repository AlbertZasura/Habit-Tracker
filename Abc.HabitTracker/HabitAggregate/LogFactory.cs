using System;
using System.Collections.Generic;

using Abc.HabitTracker.BadgeGainer;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker
{
    public class LogFactory
    {
        public static Log Create(DateTime[] Logs = null, int current_streak = 0, int longest_streak = 0, string[] days = null)
        {
            Log log;
            if (current_streak == 0)
            {
                return new Log();
            }
            return log = new Log(current_streak, longest_streak, Logs, days);

        }

        public static Log AddLog(Habit habit, DateTime currentDate, IBadgeRepository badgeRepo = null, IHabitRepository habitRepo = null)
        {
            string[] days_off = habit.DaysOff;
            string currentDay = (currentDate.DayOfWeek.ToString()).Substring(0, 3);
            int count = 0;
            DateTime[] allLog = habit.Log;
            DateTime lastDateTime = DateTime.Now;

            if (allLog.Length == 0)
            {
                lastDateTime = currentDate.AddDays(-1);
            }
            else if (allLog.Length != 0)
            {
                lastDateTime = allLog[allLog.Length - 1];
            }

            int current_streak = habit.CurrentStreak;
            int longest_streak = habit.LongestStreak;

            int diff = (currentDate - lastDateTime).Days;
            if (diff <= 0)
            {
                throw new Exception("Today, You already Logs it");
            }
            else if (diff == 1)
            {
                current_streak++;
            }
            else if (diff > 1 && diff < 7)
            {
                for (int i = 1; i < diff; i++)
                {
                    if (Array.Exists(days_off, element => element == ((lastDateTime.AddDays(i).DayOfWeek.ToString()).Substring(0, 3))) == false)
                    {
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count == (diff - 1))
                {
                    current_streak++;
                }
                else
                {
                    current_streak = 5;
                }
            }
            else
            {
                current_streak = 1;
            }

            if (longest_streak < current_streak) longest_streak = current_streak;
            if (current_streak == 0 || longest_streak == 0)
            {
                current_streak = 1;
                longest_streak = 1;
            }

            if (Array.Exists(days_off, element => element == ((currentDate.DayOfWeek.ToString()).Substring(0, 3))) == true)
            {
                count = 1;
            }

            DateTime[] logDate = { currentDate };
            string[] logDay = { currentDay };

            Log log = new Log(current_streak, longest_streak, logDate, logDay);

            //Domain Event
            TriggerHandler Dominating = new DominatingHandler(badgeRepo, new ActivityDominating());
            TriggerHandler Workaholic = new WorkaholicHandler(badgeRepo, new ActivityWorkaholic());
            TriggerHandler EpicComeback = new EpicComebackHandler(badgeRepo, new ActivityEpicComeback());

            habit.Attach(Dominating);
            habit.Attach(Workaholic);
            habit.Attach(EpicComeback);

            if (isDominating(habit.UserID, badgeRepo, current_streak)) habit.Broadcast(new Dominating(habit.UserID));
            if (isWorkaholic(habit.UserID, badgeRepo, habitRepo, count)) habit.Broadcast(new Workaholic(habit.UserID));
            if (isEpicComeback(habit.UserID, badgeRepo, current_streak, habit)) habit.Broadcast(new EpicComeback(habit.UserID));

            return log;
        }

        private static bool isDuplicate(Guid user_id, string name, IBadgeRepository badgeRepo = null)
        {
            Badge[] badges = badgeRepo.GetAllBadgeByUserId(user_id);

            for (int i = 0; i < badges.Length; i++)
            {
                if (badges[i].Name == name) return true;
            }
            return false;
        }

        private static bool isDominating(Guid user_id, IBadgeRepository badgeRepo, int CurrentStreak)
        {
            if (isDuplicate(user_id, "Dominating", badgeRepo) != true && CurrentStreak == 4) return true;
            return false;
        }

        private static bool isWorkaholic(Guid user_id, IBadgeRepository badgeRepo, IHabitRepository habitRepo, int count)
        {
            if (isDuplicate(user_id, "Workaholic", badgeRepo) != true)
            {
                Habit[] habits = habitRepo.GetAllHabitByUserId(user_id);
                for (int i = 0; i < habits.Length; i++)
                {
                    for (int j = 0; j < habits[i].LogCount; i++)
                    {
                        if (Array.Exists(habits[i].DaysOff, element => element == ((habits[i].Log[j].DayOfWeek.ToString()).Substring(0, 3))) == true)
                        {
                            count++;
                        }
                        if (count == 10)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private static bool isEpicComeback(Guid user_id, IBadgeRepository badgeRepo, int CurrentStreak, Habit habit)
        {
            DateTime[] Alldate = habit.Log;
            int diff;
            if (isDuplicate(user_id, "EpicComeback", badgeRepo) != true)
            {
                for (int i = (habit.LogCount - 1); i > 0; i--)
                {
                    diff = (Alldate[i] - Alldate[i - 1]).Days;
                    if (diff >= 10 && CurrentStreak == 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}