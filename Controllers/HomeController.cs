using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly TangyuanDbContext _db;

        public HomeController(TangyuanDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public List<PostMetadata> Index()
        {
            return _db.PostMetadata.ToList();
        }
    }
}
