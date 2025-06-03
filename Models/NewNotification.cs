using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(NotificationId))]
    public class NewNotification
    {
        public int NotificationId { get; set; }
        /// <summary>
        /// 通知类型：comment, reply, follow, mention, notice
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 目标用户ID
        /// </summary>
        public int TargetUserId { get; set; }
        /// <summary>
        /// 通知源ID
        /// </summary>
        public int SourceId { get; set; }
        /// <summary>
        /// 通知源类型：post, comment, user
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
