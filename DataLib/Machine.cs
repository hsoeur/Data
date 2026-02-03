using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
 [SwaggerSchema(Description = "Machine enregistrée dans l’environnement de production.")]
 public class Machine
 {
 [Key]
 [SwaggerSchema(Description = "Identifiant unique de la machine (clé primaire).")]
 public string MachineUid { get; set; } = default!;

 [SwaggerSchema(Description = "Libellé de la machine.")]
 public string Label { get; set; } = default!;
 }
}
