using Microsoft.EntityFrameworkCore;

namespace TangyuanBackendASP.Models
{
    /// <summary>
    /// 领域模型。所有以Base开头的属性都是中文的，作为默认值。
    /// </summary>
    [PrimaryKey(nameof(CategoryId))]
    public class Category
    {
        public required int CategoryId { get; set; }
        public required string BaseName { get; set; } = string.Empty;
        public required string BaseDescription { get; set; } = string.Empty;
    }
}
