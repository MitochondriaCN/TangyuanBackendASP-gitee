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
        public User GetSingle(int id)
        {
            return _db.User.Where<User>(u => u.UserId == id).FirstOrDefault();
        }

        //增
        // POST api/<UserController>
        /// <summary>
        /// 增加新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] CreateUserDto user)
        {
            //TODO:电话号码校验
            if (_db.User.Any<User>(u => u.PhoneNumber == user.PhoneNumber))
            {

                return Conflict("Phone number already exists");
            }
            User maxIdUser = _db.User.OrderByDescending(u => u.UserId).FirstOrDefault();
            int validId = maxIdUser == null ? 1 : maxIdUser.UserId + 1;
            User u = new()
            {
                UserId = validId,
                NickName = user.NickName,
                PhoneNumber = user.PhoneNumber
            };
            _db.User.Add(u);
            _db.SaveChanges();

            return Ok();
        }

        //改
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public IActionResult Put([FromBody] User user)
        {
            User target = _db.User.Where<User>(u => u.UserId == user.UserId).FirstOrDefault();
            if (target != null)
            {
                target.NickName = user.NickName;
                target.PhoneNumber = user.PhoneNumber;
                target.Email = user.Email;

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
            public required string PhoneNumber { get; set; }
        }
    }
}
