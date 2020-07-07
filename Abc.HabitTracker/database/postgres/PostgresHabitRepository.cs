using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker.Database.Postgres
{
    public class PostgresHabitRepository : IHabitRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public PostgresHabitRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Habit FindById(Guid id)
        {
            Habit h;
            string query = @"SELECT name,days_off,user_id,created_at FROM habits WHERE id = @id AND deleted_at is null";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string name = reader.GetString(0);
                    string[] days_off = (string[])reader.GetValue(1);
                    Guid user_id = reader.GetGuid(2);
                    DateTime created_at = reader.GetDateTime(3);
                    h = new Habit(id, name, days_off, user_id, created_at);
                }
                else
                {
                    return null;
                }
                reader.Close();
            }
            h.AddLog(getLog(id));
            return h;
        }

        public Habit[] GetAllHabitByUserId(Guid user_id)
        {
            Habit[] h = { new Habit() };
            string query = @"SELECT id,name,days_off,created_at FROM habits WHERE user_id = @user_id AND deleted_at is null";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("user_id", user_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Guid id = reader.GetGuid(0);
                    string name = reader.GetString(1);
                    string[] days_off = (string[])reader.GetValue(2);
                    DateTime created_at = reader.GetDateTime(3);
                    Habit habit = new Habit(id, name, days_off, user_id, created_at);
                    h[h.Length - 1] = habit;
                    Array.Resize(ref h, h.Length + 1);
                }
                reader.Close();
            }
            for (int i = 0; i < (h.Length - 1); i++)
            {
                h[i].AddLog(getLog(h[i].ID));
            }
            return h;
        }

        private Log getLog(Guid habit_id)
        {
            NpgsqlDateTime lastExpCreatedAt = new NpgsqlDateTime(0);
            DateTime[] AllLog = { };
            string[] Alldays = { };
            int currentStreak = 0;
            int longestStreak = 0;

            string query = "SELECT log, day, last_log_created_at FROM log_snapshot WHERE habit_id = @habit_id ORDER BY created_at DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habit_id);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        AllLog = (DateTime[])reader.GetValue(0);
                        Alldays = (string[])reader.GetValue(1);
                        lastExpCreatedAt = reader.GetTimeStamp(2);
                    }
                    reader.Close();
                }
            }

            query = "SELECT log, day FROM logs WHERE habit_id = @habit_id AND created_at > @last_exp_created_at";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habit_id);
                cmd.Parameters.AddWithValue("last_exp_created_at", lastExpCreatedAt);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime log = reader.GetDateTime(0);
                        string days = reader.GetString(1);
                        Array.Resize(ref AllLog, AllLog.Length + 1);
                        Array.Resize(ref Alldays, Alldays.Length + 1);
                        AllLog[AllLog.Length - 1] = log;
                        Alldays[Alldays.Length - 1] = days;
                    }
                    reader.Close();
                }
            }

            query = "SELECT current_streak, longest_streak FROM logs WHERE habit_id = @habit_id ORDER BY created_at DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habit_id);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentStreak = reader.GetInt32(0);
                        longestStreak = reader.GetInt32(1);
                    }
                    reader.Close();
                }
            }
            return LogFactory.Create(AllLog, currentStreak, longestStreak, Alldays);
        }

        public void Create(Habit habit)
        {
            string query = "INSERT INTO habits(id, name,days_off,user_id) VALUES(@id, @name, @days_off,@user_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", habit.ID);
                cmd.Parameters.AddWithValue("name", habit.Name);
                cmd.Parameters.AddWithValue("days_off", habit.DaysOff);
                cmd.Parameters.AddWithValue("user_id", habit.UserID);
                cmd.ExecuteNonQuery();
            }
        }

        public Habit Delete(Habit habit)
        {
            string query = "UPDATE habits SET deleted_at = CURRENT_TIMESTAMP WHERE id = @id AND deleted_at is null";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", habit.ID);
                cmd.ExecuteNonQuery();
            }
            return FindDeletedById(habit.ID);
        }

        private Habit FindDeletedById(Guid id)
        {
            Habit h;
            string query = @"SELECT name,days_off,user_id,created_at FROM habits WHERE id = @id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string name = reader.GetString(0);
                    string[] days_off = (string[])reader.GetValue(1);
                    Guid user_id = reader.GetGuid(2);
                    DateTime created_at = reader.GetDateTime(3);
                    h = new Habit(id, name, days_off, user_id, created_at);
                }
                else
                {
                    return null;
                }
                reader.Close();
            }
            h.AddLog(getLog(id));
            return h;
        }

        public void Update(Habit habit, string name, string[] days_off)
        {
            string query = "UPDATE habits SET name = @name, days_off = @days_off, updated_at = CURRENT_TIMESTAMP WHERE id = @id AND deleted_at is null";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {

                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("days_off", days_off);
                cmd.Parameters.AddWithValue("id", habit.ID);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddLog(Habit habit, DateTime currentDate)
        {
            Log currentLog = LogFactory.AddLog(habit,currentDate,new PostgresBadgeRepository(_connection, _transaction),new PostgresHabitRepository(_connection, _transaction));
            
            string query = "INSERT INTO logs(id, habit_id, log,day,current_streak,longest_streak) VALUES(@id, @habit_id, @log,@day,@current_streak,@longest_streak)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habit.ID);
                cmd.Parameters.AddWithValue("log", currentLog.Logs[0]);
                cmd.Parameters.AddWithValue("day", currentLog.Days);
                cmd.Parameters.AddWithValue("current_streak", currentLog.current_streak);
                cmd.Parameters.AddWithValue("longest_streak", currentLog.longest_streak);
                cmd.ExecuteNonQuery();
            }
            if (currentLog.longest_streak <= currentLog.current_streak) createLogSnapshot(habit.ID);

        }

        private void createLogSnapshot(Guid habit_id)
        {
            string query = "SELECT id, created_at FROM logs WHERE habit_id = @habit_id ORDER BY created_at DESC LIMIT 1";
            Guid lastLogId;
            NpgsqlDateTime lastLogCreatedAt;

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habit_id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lastLogId = reader.GetGuid(0);
                        lastLogCreatedAt = reader.GetTimeStamp(1);
                    }
                    else
                    {
                        throw new Exception("last log not found");
                    }
                }
            }

            Log allLog = getLog(habit_id);

            query = "INSERT INTO log_snapshot (id, habit_id, log,day,current_streak,longest_streak,last_log_id, last_log_created_at) VALUES(@id, @habit_id, @log,@day,@current_streak,@longest_streak, @last_log_id, @last_log_created_at)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habit_id);
                cmd.Parameters.AddWithValue("log", allLog.Logs);
                cmd.Parameters.AddWithValue("day", allLog.Days);
                cmd.Parameters.AddWithValue("current_streak", allLog.current_streak);
                cmd.Parameters.AddWithValue("longest_streak", allLog.longest_streak);
                cmd.Parameters.AddWithValue("last_log_id", lastLogId);
                cmd.Parameters.AddWithValue("last_log_created_at", lastLogCreatedAt);

                cmd.ExecuteNonQuery();
            }
        }



    }

}