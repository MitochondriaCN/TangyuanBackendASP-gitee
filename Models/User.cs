using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(UserId))]
    public class User
    {
        public int UserId { get; set; }

        public string NickName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
