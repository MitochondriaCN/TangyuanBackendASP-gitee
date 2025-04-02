using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(NotificationId))]
    public class Notification
    {
        public required int NotificationId { get; set; }
        public required int TargetUserId { get; set; }
        public required int TargetPostId { get; set; }
        public required int TargetCommentId { get; set; }
        public required int SourceCommentId { get; set; }
        public required int SourceUserId { get; set; }
        public required bool IsRead { get; set; }
        public required DateTime NotificationDateTime { get; set; }
    }
}
