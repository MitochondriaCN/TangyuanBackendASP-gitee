using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {

        private TangyuanDbContext _db;

        public CommentController(TangyuanDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 查
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            Comment c = _db.Comment.Where<Comment>(c => c.CommentId == id).FirstOrDefault();
            if (c == null)
            {
                return NotFound("No such comment.");
            }
            else
            {
                return Ok(new Comment()
                {
                    CommentId = c.CommentId,
                    ParentCommentId = c.ParentCommentId,
                    UserId = c.UserId,
                    PostId = c.PostId,
                    Content = c.Content,
                    CommentDateTime = c.CommentDateTime
                });
            }
        }

        /// <summary>
        /// 删
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Comment c = _db.Comment.Where<Comment>(c => c.CommentId == id).FirstOrDefault();
            if (c == null)
            {
                return NotFound("No such comment.");
            }
            else
            {
                _db.Comment.Remove(c);
                _db.SaveChanges();
                return Ok("Comment deleted.");
            }
        }

        /// <summary>
        /// 增
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] CreateCommentDto comment)
        {
            Comment maxIdComment = _db.Comment.OrderByDescending(c => c.CommentId).FirstOrDefault();
            Comment c = new()
            {
                CommentId = maxIdComment == null ? 1 : maxIdComment.CommentId + 1,
                ParentCommentId = comment.ParentCommentId,
                UserId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content,
                CommentDateTime = comment.CommentDateTime
            };
            _db.Comment.Add(c);
            _db.SaveChanges();
            return Ok(new
            {
                CommentId = c.CommentId
            });
        }

        public class CreateCommentDto
        {
            public required int ParentCommentId { get; set; }
            public required int UserId { get; set; }
            public required int PostId { get; set; }
            public required string Content { get; set; }
            public required DateTime CommentDateTime { get; set; }
        }
    }
}
