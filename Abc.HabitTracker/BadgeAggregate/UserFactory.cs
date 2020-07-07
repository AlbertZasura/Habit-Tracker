using System;
using System.Collections.Generic;
using Abc.HabitTracker.BadgeGainer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker
{
    public class UserFactory
    {
        public static User Create(Guid user_id)
        {
            User u;
            if (user_id == null)
            {
                throw new Exception("user ID cannot be null");
            }

            return u = new User(user_id);
            
        }
    }
}