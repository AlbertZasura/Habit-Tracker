using System;

namespace Abc.HabitTracker
{
    public interface IBadgeRepository
    {
        Badge FindById(Guid id);
        void Create(Badge badge);
        Badge FindByName(string name);
        void AddUser(Badge badge, Guid user_id);
        Badge[] GetAllBadgeByUserId(Guid user_id);
        
    }
}
