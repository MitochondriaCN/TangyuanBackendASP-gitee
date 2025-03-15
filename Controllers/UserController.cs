using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TangyuanBackendASP.Data;
using TangyuanBackendASP.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TangyuanBackendASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private TangyuanDbContext _db;

        //查
        // GET: api/<UserController>
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            User u = _db.User.Where<User>(u => u.UserId == id).FirstOrDefault();
            if (u == null)
            {
                return NotFound("No such user.");
            }
            else
            {
                return Ok(new
                {
                    UserId = u.UserId,
                    NickName = u.NickName,
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email,
                    AvatarGuid = u.AvatarGuid,
                    Bio = u.Bio,
                    ISORegionName = u.ISORegionName
                });
            }
        }

        //增
        // POST api/<UserController>
        /// <summary>
        /// 增加新用户
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] CreateUserDto user)
        {
            //TODO: 增加全局参数null校验
            try
            {
                if (DataValidator.IsPhoneNumberValid(user.PhoneNumber, user.ISORegionName) == false)
                {
                    return BadRequest("Invalid phone number.");
                }
            }
            catch(NotSupportedException e)
            {
                return BadRequest(e.Message);
            }

            if (_db.User.Any<User>(u => u.PhoneNumber == user.PhoneNumber))
            {

                return Conflict("Phone number already exists.");
            }
            User maxIdUser = _db.User.OrderByDescending(u => u.UserId).FirstOrDefault();
            int validId = maxIdUser == null ? 1 : maxIdUser.UserId + 1;
            User u = new()
            {
                UserId = validId,
                NickName = user.NickName,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                ISORegionName = user.ISORegionName,
                AvatarGuid = user.AvatarGuid
            };
            _db.User.Add(u);
            _db.SaveChanges();

            return Ok(new
            {
                UserId = validId
            });
        }

        //改
        // PUT api/<UserController>/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put([FromBody] User user)
        {
            User target = _db.User.Where<User>(u => u.UserId == user.UserId).FirstOrDefault();
            if (target != null)
            {
                target.NickName = user.NickName;
                target.PhoneNumber = user.PhoneNumber;
                target.Email = user.Email;
                target.AvatarGuid = user.AvatarGuid;
                target.Bio = user.Bio;
                target.ISORegionName = user.ISORegionName;
                target.Password = user.Password;

                _db.User.Update(target);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //删
        // DELETE api/<UserController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User target = _db.User.Where<User>(u => u.UserId == id).FirstOrDefault();
            if (target != null)
            {
                _db.User.Remove(target);
                _db.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }

        public UserController(TangyuanDbContext db)
        {
            _db = db;
        }

        public class CreateUserDto
        {
            public required string NickName { get; set; }
            public required string Password { get; set; }
            public required string PhoneNumber { get; set; }
            public required string ISORegionName { get; set; }
            public required string AvatarGuid { get; set; }
        }
    }
}
