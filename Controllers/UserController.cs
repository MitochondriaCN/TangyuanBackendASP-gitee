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
        [HttpGet("{id}")]
        public User GetSingle(int id)
        {
            return _db.User.Where<User>(u => u.UserId == id).FirstOrDefault();
        }

        //增
        // POST api/<UserController>
        [HttpPost]
        public IActionResult Post(string nickname, string phoneNumber)
        {
            if (_db.User.Any<User>(u => u.PhoneNumber == phoneNumber))
            {

                return Conflict("Phone number already exists");
            }
            int validId = _db.User.MaxBy<User, int>(User => User.UserId).UserId + 1;
            User u = new()
            {
                UserId = validId,
                NickName = nickname,
                PhoneNumber = phoneNumber
            };
            _db.User.Add(u);
            _db.SaveChanges();

            return Ok();
        }

        //改
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put([FromBody] User user)
        {
            _db.User.Update(user);
            _db.SaveChanges();
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
    }
}
