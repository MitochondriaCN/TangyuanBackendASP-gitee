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
            List<Notification> nots = _db.Notification.Where(n => n.TargetUserId == userId && !n.IsRead).ToList();
            if (nots.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(nots);
            }
        }

        [Authorize]
        [HttpGet("mark/{notificationId}")]
        public IActionResult MarkAsRead(int notificationId)
        {
            Notification n = _db.Notification.Where(n => n.NotificationId == notificationId).FirstOrDefault();
            if (n == null)
            {
                return NotFound();
            }
            else
            {
                n.IsRead = true;
                _db.SaveChanges();
                return Ok();
            }
        }
    }
}