using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Abc.HabitTracker
{
    public class Badge
    {
        //Entity
        private Guid _id;
        private string _name;
        private string _description;
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
        [JsonPropertyName("user_id")]
        public Guid UserID
        {
            get
            {
                return _user_id;
            }
        }
        [JsonPropertyName("description")]
        public string Description
        {
            get
            {
                return _description;
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

        public Badge() { }

        public Badge(Guid id, string name, string description, Guid user_id, DateTime created_at)
        {
            this._id = id;
            this._name = name;
            this._description = description;
            this._user_id = user_id;
            this._created_at = created_at;
        }
        public Badge(Guid id, string name, string description)
        {
            this._id = id;
            this._name = name;
            this._description = description;
        }

        public Badge(Guid id, string name, string description, Guid user_id) : this(name, description)
        {
            this._id = id;
            this._user_id = user_id;
        }

        public Badge(string name, string description)
        {
            this._id = Guid.NewGuid();
            this._name = name;
            this._description = description;
            this._created_at = DateTime.Now;
        }

        public static Badge NewBadge(string name, string description, Guid user_id)
        {
            return new Badge(Guid.NewGuid(), name, description, user_id);
        }

        public void AddUserId(Guid user_id)
        {
            this._user_id = user_id;
        }

        public void AddCreatedAt(DateTime created_at)
        {
            this._created_at = created_at;
        }

        public override bool Equals(object obj)
        {
            var badge = obj as Badge;
            if (badge == null) return false;

            return this._id == badge._id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
