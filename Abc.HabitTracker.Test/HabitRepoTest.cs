using System;
using Xunit;

using Npgsql;
using Abc.HabitTracker.Database.Postgres;

namespace Abc.HabitTracker.Test
{
    public class HabitRepoTest
    {
        private string connString;

        public HabitRepoTest()
        {
            connString = "Host=localhost;Username=postgres;Password=postgres;Database=habitTracker;Port=5432";
        }

        private static readonly Guid user_id = Guid.Parse("4fbb54f1-f340-441e-9e57-892329464d56");

        [Fact]
        public void CreateHabit()
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();

            IHabitRepository repo = new PostgresHabitRepository(_connection, null);
            string name = "sleep";
            string[] day_off = { "Sun" };

            Habit h = new Habit(name, day_off, user_id);
            repo.Create(h);

            Habit h2 = repo.FindById(h.ID);
            Assert.NotNull(h2);

            Assert.Equal(h.ID, h2.ID);
            Assert.Equal(h.UserID, h2.UserID);
            Assert.Equal(h.Name, h2.Name);
            Assert.Equal(h.DaysOff, h2.DaysOff);

            _connection.Close();
        }

        [Fact]
        public void GetAllHabit()
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();

            IHabitRepository repo = new PostgresHabitRepository(_connection, null);

            Habit[] h2 = repo.GetAllHabitByUserId(user_id);
            Assert.Equal(2, h2.Length);

            _connection.Close();
        }

    }
}