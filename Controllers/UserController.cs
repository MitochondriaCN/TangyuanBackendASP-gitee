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

        // GET: api/<UserController>
        [HttpGet("{id}")]
        public User GetSingle(int id)
        {
            return _db.User.Where<User>(u => u.UserId == id).FirstOrDefault();
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public UserController(TangyuanDbContext db)
        {
            _db = db;
        }
    }
}
