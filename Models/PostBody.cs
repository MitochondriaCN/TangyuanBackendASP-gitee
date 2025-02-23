namespace TangyuanBackendASP.Models
{
    public class PostBody
    {
        public int PostId { get; set; }

        public string TextContent { get; set; }

        /// <summary>
        /// 所谓相对路径，指的是从软件根目录开始的路径
        /// </summary>
        public string[]? ImageRelativePaths { get; set; }
    }
}
