using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly TangyuanDbContext _db;

        public HomeController(TangyuanDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("数据库：" + _db.Database.ProviderName
                + "\n你说得对，但是巴黎公社是一八七一年成立的，到现在九十六年了，如果巴黎公社不是失败了，" +
                "而是胜利了，那么，据我看呢，现在也已经变成资产阶级的公社了，因为法国的资产阶级不可能允" +
                "许法国的工人阶级掌握政权这么大。这是巴黎公社。苏维埃的政权形式。苏维埃政权一出来，列宁" +
                "当时很高兴，认为这是工农兵的一个伟大的创造，是无产阶级专政的新形式。但是列宁当时没有料" +
                "到这种形式工农兵可以用，资产阶级也可以用，赫鲁晓夫也可以用。那么，现在苏维埃，从列宁的" +
                "苏维埃变成了赫鲁晓夫的苏维埃。我们不是只看名称变了，问题不在名称，而在实际，不在形式，" +
                "而在内容。");
        }
    }
}
