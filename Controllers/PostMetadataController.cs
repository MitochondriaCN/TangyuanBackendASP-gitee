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

        [HttpGet]
        public PostMetadata GetSingle(int postId)
        {
            return _db.PostMetadata.Where(p => p.PostId == postId).FirstOrDefault();
        }
    }
}
