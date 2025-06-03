using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewNotificationController : Controller
    {
        private TangyuanDbContext _db;

        public NewNotificationController(TangyuanDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取一个用户的最新500条通知。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public IActionResult GetTop500ByUserId(int userId)
        {
            var notifications = _db.NewNotification
                .Where(n => n.TargetUserId == userId)
                .OrderByDescending(n => n.CreateDate)
                .Take(500)
                .ToList();
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found for the specified user.");
            }
            return Ok(notifications);
        }

        /// <summary>
        /// 将一条通知标记为已读。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("markasread/{id}")]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _db.NewNotification.Find(id);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }
            notification.IsRead = true;
            _db.SaveChanges();
            return Ok("Notification marked as read.");
        }
    }
}
