using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(UserId))]
    public class User
    {
        public required int UserId { get; set; }

        public required string NickName { get; set; }

        public required string PhoneNumber { get; set; }

        /// <summary>
        /// 用户所在区域，ISO 3166-1二位国家编码，如CN。
        /// </summary>
        public required string ISORegionName { get; set; }

        public required string AvatarGuid { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }
    }
}
