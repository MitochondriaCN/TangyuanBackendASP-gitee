using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(PostId))]
    public class PostBody
    {
        public int PostId { get; set; }

        public string TextContent { get; set; }

        //一条推文最多有三张图片，这里存储的是图片的UUID，
        //图片本身可以通过提供UUID从图片的Controller获取
        public string? Image1UUID { get; set; }

        public string? Image2UUID { get; set; }

        public string? Image3UUID { get; set; }
    }
}
