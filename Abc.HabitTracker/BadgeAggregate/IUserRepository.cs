using System;

namespace Abc.HabitTracker
{
    public interface IUserRepository
    {
        User FindById(Guid id);
        void Create(User user);
    }
}
