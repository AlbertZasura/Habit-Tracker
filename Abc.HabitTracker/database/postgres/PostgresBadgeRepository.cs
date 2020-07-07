using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker.Database.Postgres
{
    public class PostgresBadgeRepository : IBadgeRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public PostgresBadgeRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Badge FindById(Guid id)
        {
            Badge b;
            string query = @"SELECT name,description FROM badges WHERE id = @id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string name = reader.GetString(0);
                    string description = reader.GetString(1);
                    b = new Badge(id, name, description);
                }
                else
                {
                    return null;
                }
                reader.Close();
            }
            return b;
        }

        public Badge FindByName(string name)
        {
            Badge b;
            string query = @"SELECT id,description FROM badges WHERE name = @name";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("name", name);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Guid id = reader.GetGuid(0);
                    string description = reader.GetString(1);
                    b = new Badge(id, name, description);
                }
                else
                {
                    return null;
                }
                reader.Close();
            }
            return b;
        }

        public void AddUser(Badge badge, Guid user_id)
        {
            string query = "INSERT INTO achievement(user_id, badge_id) VALUES(@user_id, @badge_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("badge_id", badge.ID);
                cmd.ExecuteNonQuery();
            }
        }
        public Badge[] GetAllBadgeByUserId(Guid user_id)
        {
            Badge[] b = { new Badge() };
            List<Guid> badges = new List<Guid>();
            List<DateTime> created_ats = new List<DateTime>();

            string query = @"SELECT badge_id,created_at FROM achievement WHERE user_id = @user_id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("user_id", user_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Guid id = reader.GetGuid(0);
                    badges.Add(id);
                    DateTime created_at = reader.GetDateTime(1);
                    created_ats.Add(created_at);
                }
                reader.Close();
            }

            for (int i = 0; i < badges.Count; i++)
            {
                b[b.Length - 1] = BadgeFactory.Create(FindById(badges[i]), user_id, created_ats[i]);
                Array.Resize(ref b, b.Length + 1);
            }
            return b;
        }

        public Guid[] GetAllBadgeIdByUserId(Guid user_id)
        {
            Guid[] b = { };
            List<Guid> badges = new List<Guid>();

            string query = @"SELECT badge_id FROM achievement WHERE user_id = @user_id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("user_id", user_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Guid id = reader.GetGuid(0);
                    badges.Add(id);
                }
                reader.Close();
            }
            Array.Resize(ref b, b.Length + badges.Count);
            for (int i = 0; i < badges.Count; i++)
            {
                b[i] = badges[i];
            }
            return b;
        }

        public void Create(Badge badge)
        {
            string query = "INSERT INTO badges(id, name,description) VALUES(@id, @name, @description)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", badge.ID);
                cmd.Parameters.AddWithValue("name", badge.Name);
                cmd.Parameters.AddWithValue("description", badge.Description);
                cmd.ExecuteNonQuery();
            }
        }
    }

}