using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abc.HabitTracker.Database.Postgres
{
    public class PostgresUserRepository : IUserRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public PostgresUserRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public User FindById(Guid id)
        {
            User u;
            string query = @"SELECT id FROM users WHERE id = @id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    u = new User(id);
                }
                else
                {
                    return null;
                }
                reader.Close();
            }
            return u;
            
        }
        
        public void Create(User user)
        {
            string query = "INSERT INTO users(id) VALUES(@id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id",user.ID);
                cmd.ExecuteNonQuery();
            }
        }
    }

}