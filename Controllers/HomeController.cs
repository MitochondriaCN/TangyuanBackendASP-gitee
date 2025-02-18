using Microsoft.AspNetCore.Mvc;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            
            return Ok("陪一根");
        }
    }
}
