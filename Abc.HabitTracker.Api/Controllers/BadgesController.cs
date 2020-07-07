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
    public class BadgesController : ControllerBase
    {
        private readonly ILogger<BadgesController> _logger;
        private readonly string connString = "Host=localhost;Username=postgres;Password=postgres;Database=habitTracker;Port=5432";
        public BadgesController(ILogger<BadgesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/badges")]
        public ActionResult<IEnumerable<Badge>> All(Guid userID)
        {
            using (var uw = new PostgresUnitOfWork(connString))
            {
                if (IsUserIdValid(userID) != false)
                {
                    Badge[] b = uw.BadgeRepo.GetAllBadgeByUserId(userID);
                    Array.Resize(ref b, b.Length - 1);
                    if (b == null)
                    {
                        return NotFound("Badge not found");
                    }
                    else
                    {
                        return b;
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
