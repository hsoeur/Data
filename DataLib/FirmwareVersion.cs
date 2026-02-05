using System.ComponentModel.DataAnnotations;

namespace DataLib
{
 public class FirmwareVersion
 {
 [Key]
 public required string FirmwareVersionUid { get; set; }
 public required string ArticleUid { get; set; }
 public required string Version { get; set; }
 }
}
