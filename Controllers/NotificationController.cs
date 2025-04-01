using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly TangyuanDbContext _db;
        public NotificationController(TangyuanDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public IActionResult GetAllUnreadOfUser(int userId)
        {
            List<Notification> nots = _db.Notification.Where(n => n.UserId == userId && !n.IsRead).ToList();
            if (nots == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(nots);
            }
        }
    }
}