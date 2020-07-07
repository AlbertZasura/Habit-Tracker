using System;


namespace Abc.HabitTracker
{
    public abstract class Trigger
    {
        public Guid User_id { get; private set; }
        public Trigger(Guid user_id)
        {
            this.User_id = user_id;
        }
    }

    public class Dominating : Trigger
    {
        public Dominating(Guid user_id) : base(user_id) { }
    }

    public class Workaholic : Trigger
    {
        public Workaholic(Guid user_id) : base(user_id) { }
    }

    public class EpicComeback : Trigger
    {
        public EpicComeback(Guid user_id) : base(user_id) { }
    }
}