using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private TangyuanDbContext _db;

        public CategoryController(TangyuanDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryInfo(int id)
        {
            Category category = _db.Category.Where(c => c.CategoryId == id).FirstOrDefault();
            return category == null ? NotFound() : Ok(category);
        }

        [HttpGet("all")]
        public IActionResult GetAllCategories()
        {
            List<Category> categories = _db.Category.ToList();
            if (categories.Count == 0)
            {
                return NotFound("No category.");
            }
            else
            {
                return Ok(categories);
            }
        }

        /// <summary>
        /// 获取某个分类下的帖子数量。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("count/{id}")]
        public IActionResult GetPostCountOfCategory(int id)
        {
            Category category = _db.Category.Where(c => c.CategoryId == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound("No such category.");
            }
            else
            {
                int count = _db.PostMetadata.Where(p => p.CategoryId == id).Count();
                return Ok(count);
            }
        }

    }
}
