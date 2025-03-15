using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

namespace TangyuanBackendASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly AuthService _auth;
        private readonly TangyuanDbContext _db;

        public AuthController(AuthService auth, TangyuanDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            User u= _db.User.Where(u => u.PhoneNumber == loginDto.PhoneNumber).FirstOrDefault();
            if (u != null)
            {
                if (u.Password == loginDto.Password)
                {
                    return Ok(new
                    {
                        token = _auth.GenerateJwtToken(u.UserId)
                    });
                }
                else
                {
                    return BadRequest("Password incorrect.");
                }
            }
            else
            {
                return NotFound("User not found.");
            }
        }
    }

    public class LoginDto
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
