using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(NotificationId))]
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public int TargetPostId { get; set; }
        public int TargetCommentId { get; set; }
        public DateTime NotificationDateTime { get; set; }
    }
}
