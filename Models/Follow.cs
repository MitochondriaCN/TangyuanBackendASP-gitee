using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(SourceUserId), nameof(TargetUserId))]
    public class Follow
    {
        public required int SourceUserId { get; set; }
        public required int TargetUserId { get; set; }
        public required DateTime FollowedAt { get; set; } 
    }
}
