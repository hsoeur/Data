using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class ArticleVersion
    {
        [Key]
        public required string ArticleVersionUid { get; set; }
        public required string ArticleUid { get; set; }
        public required string FirmwareVersion { get; set; }
    }
}
