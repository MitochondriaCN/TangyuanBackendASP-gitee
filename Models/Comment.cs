using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(CommentId))]
    public class Comment
    {
        public required int CommentId { get; set; }
        public required int ParentCommentId { get; set; }
        public required int UserId { get; set; }
        public required int PostId { get; set; }
        public required string Content { get; set; }
        public required string? ImageGuid { get; set; }
        public required DateTime CommentDateTime { get; set; }
    }
}
