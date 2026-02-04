using System.ComponentModel.DataAnnotations;

namespace DataLib
{
    public class LabelType
    {
        [Key]
        public string LabelTypeUid { get; set; } = string.Empty;
    }
}
