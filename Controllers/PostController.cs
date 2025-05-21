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

        //查最新公告，公告的sectionId是0
        [HttpGet("metadata/notice")]
        public IActionResult GetNewestNotice()
        {
            PostMetadata noticeMetadata = _db.PostMetadata.Where(p => p.SectionId == 0).OrderByDescending(p => p.PostDateTime).FirstOrDefault();
            if (noticeMetadata == null)
            {
                return NotFound("No notice.");
            }
            else
            {
                return Ok(noticeMetadata);
            }
        }

        /// <summary>
        /// 随机推荐算法。该算法的作用是：从数据库中获取一个帖子元数据的集合，该集合满足下列条件：
        /// 1. 该集合大小为count；
        /// 2. 该集合中每个元素的DateTime都在当前时间的前一星期内；
        /// 3. 多次调用该API时，返回的集合不必相同。
        /// 如果在满足2.的基础上，不足以找到count个元素，则随机抽取剩余元素，填充到count个元素。
        /// </summary>
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


            DateTime oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            List<PostMetadata> untakeMetadata = _db.PostMetadata
                .Where(p => p.PostDateTime >= oneWeekAgo)
                .ToList();
            List<PostMetadata> metadata = untakeMetadata.OrderBy(p => Guid.NewGuid()).Take(count).ToList();

            if (metadata.Count < count)
            {
                //如果在满足2.的基础上，不足以找到count个元素，则随机抽取剩余元素，填充到count个元素
                List<PostMetadata> allMetadata = _db.PostMetadata.ToList();
                Random random = new Random();
                while (metadata.Count < count)
                {
                    int randomIndex = random.Next(allMetadata.Count);
                    PostMetadata randomPost = allMetadata[randomIndex];
                    if (!metadata.Contains(randomPost))
                    {
                        metadata.Add(randomPost);
                    }
                }
            }
            return Ok(metadata);
        }

        //查某领域24小时内发表的帖子数
        [HttpGet("count/category/24h/{categoryId}")]
        public IActionResult Get24HPostCountByCategoryId(int categoryId)
        {
            if (!_db.Category.Any(p => p.CategoryId == categoryId))
            {
                return BadRequest("No such category.");
            }

            DateTime since = DateTime.UtcNow.AddDays(-1);
            int count = _db.PostMetadata
                .Where(p => p.CategoryId == categoryId && p.PostDateTime >= since)
                .Count();

            return Ok(count);
        }


        /// <summary>
        /// 获取某个领域下最近一周的帖子数量。
        /// </summary>
        [HttpGet("count/category/7d/{categoryId}")]
        public IActionResult GetWeeklyNewPostCountOfCategory(int categoryId)
        {
            Category category = _db.Category.Where(c => c.CategoryId == categoryId).FirstOrDefault();
            if (category == null)
            {
                return NotFound("No such category.");
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                DateTime lastWeek = now.AddDays(-7);
                int count = _db.PostMetadata.Where(p => p.CategoryId == categoryId && p.PostDateTime >= lastWeek).Count();
                return Ok(count);
            }
        }

        //查某领域所有帖子元数据
        [HttpGet("metadata/category/{categoryId}")]
        public IActionResult GetMetadatasByCategoryId(int categoryId)
        {
            if (!_db.Category.Any(p => p.CategoryId == categoryId))
            {
                return BadRequest("No such category.");
            }
            List<PostMetadata> metadata = _db.PostMetadata
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => p.PostDateTime)
                .Take(500)//取前500条
                .ToList();
            return Ok(metadata);
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
                PostDateTime = DateTime.UtcNow,//这里使用UTC时间，避免时区问题，弃用dto中的PostDateTime
                SectionId = post.SectionId,
                CategoryId = post.CategoryId,
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
            public int CategoryId { get; set; }
            public bool IsVisible { get; set; }
        }
    }
}
