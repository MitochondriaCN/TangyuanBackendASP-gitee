namespace TangyuanBackendASP.Models
{
    public class Comment
    {
        public required int CommentId { get; set; }
        public required string ParentCommentId { get; set; }
        public required int UserId { get; set; }
        public required int PostId { get; set; }
        public required string Content { get; set; }
        public required DateTime CommentDateTime { get; set; }
    }
}
