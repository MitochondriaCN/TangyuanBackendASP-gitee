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
        /// 1. 获取用户给定的已经获取的帖子ID。创建推荐帖子列表suggestedPosts。
        /// 2. 令w=0，随机使用[7(w+1),7w]天前范围内的帖子填充suggestedPosts，并且排除用户已经获取的帖子ID。
        /// 3. 如果suggestedPosts的数量小于20，令w=w+1，重复步骤2。
        /// 4. 如果当前w=24，仍然没有获取到20条帖子元数据，则返回404。
        /// </summary>
        /// <param name="exceptedPosts"></param>
        /// <returns></returns>
        [HttpPost("postmetadata/{sectionId}")]
        public IActionResult PostMetadataCommon(int sectionId, [FromBody] List<int> exceptedPostIds)
        {
            // 1. 初始化推荐帖子列表
            var suggestedPosts = new List<int>();
            // 2. 从w=0开始循环，直到w=24
            for (int w = 0; w < 25; w++)
            {
                // 计算时间区间：[7(w+1), 7w]天前
                var now = DateTime.UtcNow;
                var end = now.AddDays(-7 * w);
                var start = now.AddDays(-7 * (w + 1));
                // 查询该区间内的所有帖子（只选IsVisible为true）
                var query = _db.PostMetadata
                    .Where(p => p.IsVisible
                    && p.PostDateTime >= start
                    && p.PostDateTime < end
                    && p.SectionId == sectionId);
                // 排除用户已经获取的帖子ID
                if (exceptedPostIds != null && exceptedPostIds.Count > 0)
                {
                    query = query.Where(p => !exceptedPostIds.Contains(p.PostId));
                }
                // 将查询结果转换为列表
                var posts = query.Select(p => p.PostId).ToList();
                // 随机选取最多20条
                var rng = new Random();
                var selectedPosts = posts.OrderBy(x => rng.Next()).Take(20 - suggestedPosts.Count).ToList();
                // 将选中的帖子添加到推荐列表中
                suggestedPosts.AddRange(selectedPosts);
                // 如果已经获取到20条，返回结果
                if (suggestedPosts.Count >= 20)
                {
                    //返回List<PostMetadata>
                    return Ok(_db.PostMetadata
                        .Where(p => suggestedPosts.Contains(p.PostId))
                        .ToList());
                }
            }
            // 如果循环结束后仍然没有获取到20条，返回404 Not Found
            return NotFound("No enough posts found.");
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
