using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {

        private readonly TangyuanDbContext _db;

        public SearchController(TangyuanDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 全文搜索帖子内容
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>PostMetadata的集合</returns>
        [HttpGet("post/{keyword}")]
        public IActionResult SearchPostByKeyword(string keyword)
        {
            // 使用参数化查询防止SQL注入
            var postIds = _db.PostBody
                .FromSqlRaw("SELECT * FROM PostBody WHERE MATCH (TextContent) AGAINST ({0})", keyword)
                .Select(pb => pb.PostId)
                .ToList();
            if (postIds.Any())
            {

                var posts = _db.PostMetadata
                    .Where(pm => postIds.Contains(pm.PostId))
                    .ToList();

                return Ok(posts);
            }
            else
            {
                return NotFound("No posts found matching the keyword.");
            }
        }

        /// <summary>
        /// 全文搜索用户昵称
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns>User的集合</returns>
        [HttpGet("user/{nickname}")]
        public IActionResult SearchUserByNickname(string nickname)
        {
            // 使用参数化查询防止SQL注入
            var users = _db.User
                .FromSqlRaw("SELECT * FROM User WHERE MATCH (NickName) AGAINST ({0})", nickname)
                .ToList();
            if (users.Any())
            {
                return Ok(users);
            }
            else
            {
                return NotFound("No users found matching the nickname.");
            }
        }

        /// <summary>
        /// 全文搜索评论内容
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>Comment的集合</returns>
        [HttpGet("comment/{keyword}")]
        public IActionResult SearchCommentByKeyword(string keyword)
        {
            // 使用参数化查询防止SQL注入
            var commentIds = _db.Comment
                .FromSqlRaw("SELECT * FROM Comment WHERE MATCH (Content) AGAINST ({0})", keyword)
                .Select(c => c.CommentId)
                .ToList();
            if (commentIds.Any())
            {
                var comments = _db.Comment
                    .Where(c => commentIds.Contains(c.CommentId))
                    .ToList();
                return Ok(comments);
            }
            else
            {
                return NotFound("No comments found matching the keyword.");
            }
        }
    }
}
