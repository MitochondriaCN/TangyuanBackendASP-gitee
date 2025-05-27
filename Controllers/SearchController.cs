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
    }
}
