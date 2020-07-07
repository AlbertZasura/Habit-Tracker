using System;
using Xunit;

using Npgsql;
using Abc.HabitTracker.Database.Postgres;

namespace Abc.HabitTracker.Test
{
    public class UnitOfWorkTest
    {
        private string connString;

        public UnitOfWorkTest()
        {
            connString = "Host=localhost;Username=postgres;Password=postgres;Database=habitTracker;Port=5432";
        }

        private static readonly Guid id = Guid.Parse("ce510da5-257f-40f2-801e-aa0ad6fc4c6a");
        private static readonly Guid user_id = Guid.Parse("4fbb54f1-f340-441e-9e57-892329464d56");
        [Fact]
        public void LogTest()
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                for (int i = 0; i < 1; i++)
                {
                    DateTime curentDate = DateTime.Now.AddDays(i+3);
                    Habit h = uw.HabitRepo.FindById(id);
                    uw.HabitRepo.AddLog(h, curentDate);
                }
                uw.Commit();
                Badge[] badges = uw.BadgeRepo.GetAllBadgeByUserId(user_id);
                Assert.Equal("Dominating", badges[0].Name);
            }
        }

        [Fact]
        public void UserNotValid()
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                User u = uw.UserRepo.FindById(id);
                Assert.Null(u);
            }
        }

        [Fact]
        public void ValidUser()
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                User u = uw.UserRepo.FindById(user_id);
                Assert.Equal(user_id, u.ID);
            }
        }

        [Fact]
        public void createNewDominatingBadge()
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                Badge b = new Badge("Dominating", "4+ streak");
                uw.BadgeRepo.Create(b);
                uw.Commit();
                Assert.Equal("Dominating", b.Name);
            }
        }
    }
}