using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class Sequence
    {
        [Key]
        public string SequenceUid { get; set; }
        [ForeignKey(nameof(Article))]
        public string Product { get; set; }
        public Article? Article { get; set; }
        public int OperationNumber { get; set; }
        public string File { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
