using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TangyuanBackendASP.Models
{
    [PrimaryKey(nameof(PostId))]
    public class PostMetadata
    {
        
        public int PostId { get; set; }

        public int UserId {  get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PostDateTime {  get; set; }

        /// <summary>
        /// 所处板块的ID
        /// </summary>
        public int SectionId {  get; set; }

        public bool IsVisible { get; set; }
    }
}
