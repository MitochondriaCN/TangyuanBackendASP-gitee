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

            _db.SaveChanges();

            //无论如何，楼主都会收到一条新评论通知
            _db.NewNotification.Add(new NewNotification
            {
                NotificationId = _db.NewNotification.OrderByDescending(n => n.NotificationId).FirstOrDefault()?.NotificationId + 1 ?? 1,
                Type = "comment",
                TargetUserId = _db.PostMetadata.Where(p => p.PostId == c.PostId).FirstOrDefault().UserId, //目标用户为楼主，claims that this could never be null because of the previous check
                SourceId = c.CommentId,
                SourceType = "comment",
                IsRead = false,
                CreateDate = DateTime.UtcNow
            });

            _db.SaveChanges();

            //如果是一条回复
            if (c.ParentCommentId != 0)
            {
                if (_db.Comment.Find(c.ParentCommentId).UserId != c.UserId &&
                    _db.Comment.Find(c.ParentCommentId).UserId != _db.PostMetadata.Where(p => p.PostId == c.PostId).FirstOrDefault().UserId)
                {
                    //那么对被回复者发送通知。注意到回复自己或楼主时，不需要给自己或楼主发送通知。
                    _db.NewNotification.Add(new NewNotification
                    {
                        NotificationId = _db.NewNotification.OrderByDescending(n => n.NotificationId).FirstOrDefault()?.NotificationId + 1 ?? 1,
                        Type = "reply",
                        TargetUserId = _db.Comment.Where(c => c.CommentId == comment.ParentCommentId).FirstOrDefault().UserId,
                        SourceId = c.CommentId,
                        SourceType = "comment",
                        IsRead = false,
                        CreateDate = DateTime.UtcNow
                    });

                    _db.SaveChanges();
                }

                //同时，对已经回复过同一条父评论的用户发送通知，注意到不需要给楼主发送通知。
                var usersWhoReplied = _db.Comment
                    .Where(c => c.ParentCommentId == comment.ParentCommentId && c.UserId != comment.UserId
                    && c.UserId != _db.PostMetadata.Where(p => p.PostId == c.PostId).FirstOrDefault().UserId)
                    .Select(c => c.UserId)
                    .Distinct()
                    .ToList();
                foreach (var userId in usersWhoReplied)
                {
                    _db.NewNotification.Add(new NewNotification
                    {
                        NotificationId = _db.NewNotification.OrderByDescending(n => n.NotificationId).FirstOrDefault()?.NotificationId + 1 ?? 1,
                        Type = "reply",
                        TargetUserId = userId,
                        SourceId = c.CommentId,
                        SourceType = "comment",
                        IsRead = false,
                        CreateDate = DateTime.UtcNow
                    });
                    _db.SaveChanges();
                }
            }

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
