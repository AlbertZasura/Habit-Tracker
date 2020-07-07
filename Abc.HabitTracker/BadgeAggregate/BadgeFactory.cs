using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker
{
    public class BadgeFactory
    {
        public static Badge Create(Badge badge, Guid user_id, DateTime created_at)
        {
            Badge b;
            if (user_id == null)
            {
                return badge;
            }
            return b = new Badge(badge.ID, badge.Name, badge.Description, user_id, created_at);
        }
    }
}