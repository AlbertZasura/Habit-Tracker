using System;

namespace Abc.HabitTracker
{
    public interface IHabitRepository
    {
        Habit FindById(Guid id);
        Habit[] GetAllHabitByUserId(Guid user_id);
        void Create(Habit habit);
        Habit Delete(Habit habit);
        void Update(Habit habit,string name, string[] days_off);
        void AddLog(Habit habit, DateTime currentDate);
    }
}