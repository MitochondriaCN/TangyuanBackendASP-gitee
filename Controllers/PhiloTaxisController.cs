using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;

namespace TangyuanBackendASP.Controllers
{
    /// <summary>
    /// PhiloTaxis™ 循秩™ 推荐控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PhiloTaxisController : Controller
    {
        private readonly TangyuanDbContext _db;

        public PhiloTaxisController(TangyuanDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取随机20条帖子元数据。此算法用户无关、状态无关，每次调用返回结果不一定相同。
        /// 选取方法：
        /// 1. 在过去[7(w+1),7w]天内的所有帖子中，随机选取20条。
        /// 2. 如果这个时间区间内的总帖子数不足20条，那么有多少条抽多少条，可以是0条。
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet("postmetadata/earlierday/{w}")]
        public IActionResult GetSuggestedPostMetadataInWeekEarlier(int w)
        {
            // 1. 计算时间区间：[7(w+1), 7w]天前
            var now = DateTime.UtcNow;
            var end = now.AddDays(-7 * w);
            var start = now.AddDays(-7 * (w + 1));

            // 2. 查询该区间内的所有帖子（只选IsVisible为true）
            var query = _db.PostMetadata
                .Where(p => p.IsVisible && p.PostDateTime >= start && p.PostDateTime < end);

            // 3. 取出所有PostMetadata到内存
            var posts = query.ToList();

            // 4. 随机选取最多20条
            var rng = new Random();
            var result = posts.OrderBy(x => rng.Next()).Take(20).ToList();

            // 5. 返回结果（可以为0条）
            return Ok(result);
        }
    }
}
