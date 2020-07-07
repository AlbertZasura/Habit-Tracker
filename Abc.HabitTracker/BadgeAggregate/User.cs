using System;

using System.Collections.Generic;

namespace Abc.HabitTracker
{
    public class User
    {
        //Entity
        private Guid _id;

        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        public User(Guid id) : this()
        {
            this._id = id;
        }

        public User()
        {
            _id = Guid.NewGuid();
        }

        public static User NewUser()
        {
            return new User();
        }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            if (user == null) return false;

            return this._id == user._id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
