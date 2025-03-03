using Microsoft.AspNetCore.Mvc;

namespace TangyuanBackendASP.Controllers
{
    /// <summary>
    /// 管理图片的上传，下载不需要管理，因为可以通过UseStaticFiles()中间件直接访问。
    /// </summary>
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        /// <summary>
        /// 上传图片。使用时，一定要严格判断文件格式，严禁某些SB上传非JPEG格式的文件。
        /// </summary>
        /// <returns></returns>
        [HttpPost("image/uploadjpg")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is null or empty.");
            }

            string guid = Guid.NewGuid().ToString();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", guid);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { guid });
        }
    }
}
