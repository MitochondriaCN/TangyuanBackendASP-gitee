using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostMetadataController : Controller
    {
        private readonly TangyuanDbContext _db;

        public PostMetadataController(TangyuanDbContext db)
        {
            _db = db;
        }

        //查
        [HttpGet("{id}")]
        public PostMetadata GetSingle(int id)
        {
            return _db.PostMetadata.Where(p => p.PostId == id).FirstOrDefault();
        }

        //增
        [HttpPost]
        public IActionResult Post([FromBody] CreatPostMetadataDto post)
        {
            PostMetadata maxIdPost = _db.PostMetadata.OrderByDescending(p => p.PostId).FirstOrDefault();
            int validId = maxIdPost == null ? 1 : maxIdPost.PostId + 1;
            PostMetadata p = new()
            {
                PostId = validId,
                UserId = post.UserId,
                PostDateTime = post.PostDateTime,
                SectionId = post.SectionId,
                Title = post.Title,
                IsVisible = post.IsVisible
            };
            _db.PostMetadata.Add(p);
            _db.SaveChanges();
            return Ok();
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
            _db.PostMetadata.Remove(p);
            _db.SaveChanges();
            return Ok();
        }

        //我们姑且认为推文是不需要提供修改API的

        public class CreatPostMetadataDto
        {
            public int UserId { get; set; }
            public DateTime PostDateTime { get; set; }
            public int SectionId { get; set; }
            public string Title { get; set; }
            public bool IsVisible { get; set; }
        }
    }
}
