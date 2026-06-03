using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Unite de mesure utilisee par les tolerances et les mesures de production.")]
    public class Unit
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l'unite (ex: Volt, Celsius, Ohm, Unknown).")]
        public string UnitUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Libelle lisible de l'unite de mesure.")]
        public string Label { get; set; } = string.Empty;
    }
}
