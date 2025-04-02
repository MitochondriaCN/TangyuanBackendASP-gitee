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
                    ImageGuid = c.ImageGuid,
                    CommentDateTime = c.CommentDateTime
                });
            }
        }

        /// <summary>
        /// 查帖子下所有评论
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("post/{postId}")]
        public IActionResult GetCommentForPost(int postId)
        {
            if (!_db.PostMetadata.Any(p => p.PostId == postId))
            {
                return NotFound("No such post.");
            }
            List<Comment> comments = _db.Comment.Where<Comment>(c => c.PostId == postId).ToList();
            if (comments.Count == 0)
            {
                return NotFound("No comments.");
            }
            return Ok(comments);
        }

        /// <summary>
        /// 查评论下所有子评论
        /// </summary>
        [HttpGet("sub/{parentCommentId}")]
        public IActionResult GetSubComment(int parentCommentId)
        {
            if (!_db.Comment.Any(c => c.CommentId == parentCommentId))
            {
                return NotFound("Parent comment doesn't exist.");
            }
            List<Comment> comments = _db.Comment.Where<Comment>(c => c.ParentCommentId == parentCommentId).ToList();
            if (comments.Count == 0)
            {
                return NotFound("No sub comments.");
            }
            return Ok(comments);
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
                ImageGuid = comment.ImageGuid,
                CommentDateTime = DateTime.UtcNow //这里使用UTC时间，避免时区问题，弃用dto中的CommentDateTime
            };
            _db.Comment.Add(c);

            _db.Notification.Add(new Notification
            {
                NotificationId = _db.Notification.OrderByDescending(n => n.NotificationId).FirstOrDefault().NotificationId + 1,
                TargetUserId = _db.PostMetadata.Where(p => p.PostId == comment.PostId).FirstOrDefault().UserId,
                TargetPostId = c.PostId,
                TargetCommentId = c.ParentCommentId,
                SourceCommentId = c.CommentId,
                SourceUserId = c.UserId,
                IsRead = false,
                NotificationDateTime = DateTime.UtcNow
            });

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
            public string? ImageGuid { get; set; }
            public required DateTime CommentDateTime { get; set; }
        }
    }
}
