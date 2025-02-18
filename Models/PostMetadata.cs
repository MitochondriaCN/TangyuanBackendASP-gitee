using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TangyuanBackendASP.Models
{
    public class PostMetadata
    {
        private int PostId { get; set; }

        private int UserId {  get; set; }

        [DataType(DataType.DateTime)]
        private DateTime PostDateTime {  get; set; }

        private int SectionId {  get; set; }
    }
}
