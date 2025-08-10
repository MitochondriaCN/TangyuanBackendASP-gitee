using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocialController : Controller
    {
        readonly TangyuanDbContext _db;

        public SocialController(TangyuanDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取用户的所有关注者ID列表。
        /// </summary>
        /// <returns></returns>
        [HttpGet("followee/{sourceUserId}")]
        public IActionResult GetAllFolloweeUserId(int sourceUserId)
        {
            List<int> followee = _db.Follow
                .Where(f => f.SourceUserId == sourceUserId)
                .Select(f => f.TargetUserId)
                .ToList();
            if (followee.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(followee);
            }
        }

        /// <summary>
        /// 获取用户的所有粉丝ID列表。
        /// </summary>
        /// <returns></returns>
        [HttpGet("follower/{sourceUserId}")]
        public IActionResult GetAllFollowerUserId(int sourceUserId)
        {
            List<int> follower = _db.Follow
                .Where(f => f.TargetUserId == sourceUserId)
                .Select(f => f.SourceUserId)
                .ToList();
            if (follower.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(follower);
            }
        }

        /// <summary>
        /// 创建一个新的关注关系。
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("follow/create")]
        public IActionResult CreateFollow([FromBody] FollowDto dto)
        {
            if (dto == null || dto.SourceUserId <= 0 || dto.TargetUserId <= 0)
            {
                return BadRequest("Invalid follow data.");
            }
            // 检查是否已经存在相同的关注关系
            var existingFollow = _db.Follow
                .FirstOrDefault(f => f.SourceUserId == dto.SourceUserId && f.TargetUserId == dto.TargetUserId);
            if (existingFollow != null)
            {
                return Conflict("You are already following this user.");
            }
            //禁止自己关注自己
            if (dto.SourceUserId == dto.TargetUserId)
            {
                return BadRequest("You cannot follow yourself.");
            }
            Follow follow = new Follow
            {
                SourceUserId = dto.SourceUserId,
                TargetUserId = dto.TargetUserId,
                FollowedAt = DateTime.UtcNow // 使用UTC时间
            };
            _db.Follow.Add(follow);

            //发送通知
            NewNotification notification = new NewNotification
            {
                Type = "follow",
                TargetUserId = dto.TargetUserId,
                SourceId = dto.SourceUserId,
                SourceType = "user",
                IsRead = false,
                CreateDate = DateTime.UtcNow
            };
            _db.NewNotification.Add(notification);

            _db.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPost("follow/delete")]
        public IActionResult DeleteFollow([FromBody] FollowDto dto)
        {
            if (dto == null || dto.SourceUserId <= 0 || dto.TargetUserId <= 0)
            {
                return BadRequest("Invalid follow data.");
            }
            // 查找要删除的关注关系
            var follow = _db.Follow
                .FirstOrDefault(f => f.SourceUserId == dto.SourceUserId && f.TargetUserId == dto.TargetUserId);
            if (follow == null)
            {
                return NotFound("Follow relationship not found.");
            }
            _db.Follow.Remove(follow);
            _db.SaveChanges();
            return Ok();
        }

        public class FollowDto
        {
            public int SourceUserId { get; set; }
            public int TargetUserId { get; set; }
        }
    }
}
