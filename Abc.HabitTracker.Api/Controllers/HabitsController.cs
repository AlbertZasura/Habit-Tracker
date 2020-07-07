using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Abc.HabitTracker.Database.Postgres;

namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly ILogger<HabitsController> _logger;
        private readonly string connString = "Host=localhost;Username=postgres;Password=postgres;Database=habitTracker;Port=5432";
        public HabitsController(ILogger<HabitsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/habits")]
        public ActionResult<IEnumerable<Habit>> All(Guid userID)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Habit[] h = uw.HabitRepo.GetAllHabitByUserId(userID);
                    Array.Resize(ref h, h.Length - 1);
                    if (h.Length <= 0)
                    {
                        return NotFound("there is no habit");
                    }
                    else
                    {
                        return h;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        [HttpGet("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> Get(Guid userID, Guid id)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Habit h = uw.HabitRepo.FindById(id);
                    if (h == null || h.UserID != userID)
                    {
                        return NotFound("habit not found");
                    }
                    else
                    {
                        return h;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        [HttpPost("api/v1/users/{userID}/habits")]
        public ActionResult<Habit> AddNewHabit(Guid userID, [FromBody] RequestData data)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Habit h = new Habit(data.Name, data.DaysOff, userID);
                    uw.HabitRepo.Create(h);
                    uw.Commit();
                    if (h == null)
                    {
                        return NotFound("habit not found");
                    }
                    else
                    {
                        return h;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        [HttpPut("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> UpdateHabit(Guid userID, Guid id, [FromBody] RequestData data)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Habit h = uw.HabitRepo.FindById(id);
                    if (h == null || h.UserID != userID)
                    {
                        return NotFound("habit not found");
                    }
                    else
                    {
                        uw.HabitRepo.Update(h, data.Name, data.DaysOff);
                        h = uw.HabitRepo.FindById(id);
                        uw.Commit();
                        return h;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        [HttpDelete("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> DeleteHabit(Guid userID, Guid id)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Habit h = uw.HabitRepo.FindById(id);
                    if (h == null || h.UserID != userID)
                    {
                        return NotFound("habit not found");
                    }
                    else
                    {
                        h = uw.HabitRepo.Delete(h);
                        uw.Commit();
                        return h;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        [HttpPost("api/v1/users/{userID}/habits/{id}/logs")]
        public ActionResult<Habit> Log(Guid userID, Guid id)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    DateTime curentDate = DateTime.Now;
                    Habit h = uw.HabitRepo.FindById(id);
                    if (h == null || h.UserID != userID)
                    {
                        return NotFound("habit not found");
                    }
                    else
                    {
                        uw.HabitRepo.AddLog(h, curentDate);
                        Habit newHabit = uw.HabitRepo.FindById(id);
                        uw.Commit();
                        return newHabit;
                    }
                }
                else
                {
                    return NotFound("user not found");
                }
            }
        }

        private Boolean IsUserIdValid(Guid userID)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                User u = uw.UserRepo.FindById(userID);
                if (u == null)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
