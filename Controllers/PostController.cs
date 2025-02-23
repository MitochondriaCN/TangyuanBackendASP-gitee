using Microsoft.AspNetCore.Mvc;
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
        public PostMetadata GetSingleMetadata(int id)
        {
            return _db.PostMetadata.Where(p => p.PostId == id).FirstOrDefault();
        }

        //查内容
        [HttpGet("body/{id}")]
        public PostBody GetSingleBody(int id)
        {
            return _db.PostBody.Where(p => p.PostId == id).FirstOrDefault();
        }

        //增元数据
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
            return Ok();
        }

        //增内容
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
            if(pb == null)
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
