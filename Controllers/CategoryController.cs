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
    }
}
