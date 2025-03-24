using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly TangyuanDbContext _db;

        public PostController(TangyuanDbContext db)
        {
            _db = db;
        }

        //查元数据
        [HttpGet("metadata/{id}")]
        public IActionResult GetSingleMetadata(int id)
        {
            PostMetadata metadata = _db.PostMetadata.Where(p => p.PostId == id).FirstOrDefault();
            if (metadata == null)
            {
                return NotFound("No such post.");
            }
            else
            {
                return Ok(metadata);
            }
        }

        //查随机条元数据
        [HttpGet("metadata/random/{count}")]
        public IActionResult GetRandomMetadata(int count)
        {
            if (count <= 0 || count > 10)
            {
                return BadRequest("Count should be between 1 and 10.");
            }
            if (count > _db.PostMetadata.Count())
            {
                return BadRequest("Count should be less than the total number of posts.");
            }
            return Ok(_db.PostMetadata.OrderBy(p => EF.Functions.Random()).Take(count).ToList());
        }

        //查内容
        [HttpGet("body/{id}")]
        public IActionResult GetSingleBody(int id)
        {
            PostBody pb = _db.PostBody.Where(p => p.PostId == id).FirstOrDefault();
            if (pb == null)
            {
                return NotFound("No such post.");
            }
            else
            {
                return Ok(pb);
            }
        }

        //根据用户ID查所有帖子Metadata
        [HttpGet("metadata/user/{userId}")]
        public IActionResult GetMetadatasByUserId(int userId)
        {
            if (!_db.User.Any(p => p.UserId == userId))
            {
                return BadRequest("No such user.");
            }
            return Ok(_db.PostMetadata.Where(p => p.UserId == userId).ToList());
        }


        //增元数据
        [Authorize]
        [HttpPost("metadata")]
        public IActionResult CreatePostMetadata([FromBody] CreatPostMetadataDto post)
        {
            PostMetadata maxIdPost = _db.PostMetadata.OrderByDescending(p => p.PostId).FirstOrDefault();
            int validId = maxIdPost == null ? 1 : maxIdPost.PostId + 1;
            PostMetadata p = new()
            {
                PostId = validId,
                UserId = post.UserId,
                PostDateTime = post.PostDateTime,
                SectionId = post.SectionId,
                IsVisible = post.IsVisible
            };
            _db.PostMetadata.Add(p);
            _db.SaveChanges();
            //返回匿名类型，包含新建的推文ID
            return Ok(new { PostId = validId });
        }

        //增内容
        [Authorize]
        [HttpPost("body")]
        public IActionResult CreatePostBody([FromBody] PostBody post)
        {
            if (_db.PostMetadata.Any(p => p.PostId == post.PostId)
                && !_db.PostBody.Any(p => p.PostId == post.PostId))
            {
                _db.PostBody.Add(post);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("No corresponding metadata or the body already exists");
            }
        }

        //删
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            PostMetadata p = _db.PostMetadata.Where(p => p.PostId == id).FirstOrDefault();
            if (p == null)
            {
                return NotFound();
            }

            //删元数据
            _db.PostMetadata.Remove(p);

            //删内容
            PostBody pb = _db.PostBody.Where(p => p.PostId == id).FirstOrDefault();
            if (pb == null)
            {
                //光有元数据没内容是服务器的问题，虽然不该发生，但毕竟没什么影响，所以我们不返回500，
                //但要在信息中说明这个问题
                _db.SaveChanges();
                return Ok("The metadata has been deleted, but the body does not exist");

            }
            else
            {
                _db.PostBody.Remove(pb);
            }

            _db.SaveChanges();
            return Ok();
        }

        //我们姑且认为推文是不需要提供修改API的

        public class CreatPostMetadataDto
        {
            public int UserId { get; set; }
            public DateTime PostDateTime { get; set; }
            public int SectionId { get; set; }
            public bool IsVisible { get; set; }
        }
    }
}
