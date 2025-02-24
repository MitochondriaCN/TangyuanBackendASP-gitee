using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(UserId))]
    public class User
    {
        public required int UserId { get; set; }

        public required string NickName { get; set; }

        public required string PhoneNumber { get; set; }

        public required string ISORegionName { get; set; }


        public string? Email { get; set; }
    }
}
