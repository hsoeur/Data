namespace Api.Data.Dtos
{
 public class LabelTemplateDto
 {
 public required string Product { get; set; }
 public required string LabelTypeUid { get; set; }
 public required string TemplateFile { get; set; }
 public required string Photo { get; set; }
 public required DateTimeOffset StartDate { get; set; }
 public required DateTimeOffset EndDate { get; set; }
 public required string UserUid { get; set; }
 }
}
