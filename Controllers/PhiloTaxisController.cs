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
        /// PhiloTaxis™ 循秩™ 帖子推荐主算法。最理想情况下，给出20条帖子元数据。
        /// 1. 获取用户给定的已经获取的帖子ID。
        /// 2. 从w=0开始，调用GetSuggested...获取每个w的帖子元数据，直到获取到20条为止。
        /// 3. 在2.的过程中，排除用户已经获取的帖子ID。
        /// 4. 如果在w>24的情况下仍然没有获取到20条，那么返回已获取的帖子ID列表。
        /// </summary>
        /// <param name="exceptedPosts"></param>
        /// <returns></returns>
        [HttpPost("postmetadata")]
        public IActionResult PostMetadataCommon(List<int> exceptedPostIds)
        {
            // 1. 获取用户给定的已经获取的帖子ID
            var exceptedPosts = exceptedPostIds ?? new List<int>();
            // 2. 从w=0开始，调用GetSuggested...获取每个w的帖子元数据，直到获取到20条为止。
            var suggestedPosts = new List<Models.PostMetadata>();
            for (int w = 0; w <= 24; w++)
            {
                var result = GetSuggestedPostMetadataInWeekEarlier(w);
                if (result is OkObjectResult okResult)
                {
                    var posts = okResult.Value as List<Models.PostMetadata>;
                    if (posts != null)
                    {
                        // 3. 排除用户已经获取的帖子ID
                        posts = posts.Where(p => !exceptedPosts.Contains(p.PostId)).ToList();
                        suggestedPosts.AddRange(posts);
                    }
                }
                // 如果已经获取到20条，退出循环
                if (suggestedPosts.Count >= 20)
                {
                    break;
                }
            }
            // 4. 如果一条帖子都没有获取到，返回404
            if (suggestedPosts.Count == 0)
            {
                return NotFound("No suggested posts found.");
            }
            // 返回前20条建议的帖子元数据
            return Ok(suggestedPosts.Take(20).ToList());
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
