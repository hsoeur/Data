using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    public class MeasurementTraceability
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la mesure enregistrée (clé primaire).")]
        public string MeasurementTraceabilityUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant du produit mesuré.")]
        public string ProductUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de série du produit concerné.")]
        public int SerialNumber { get; set; }

        [SwaggerSchema(Description = "Libellé ou nom de la mesure (ex: Tension, Température, etc.).")]
        public string Label { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Valeur mesurée.")]
        public string Value { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Classification de la mesure (ex: OK, NOK, Alerte).")]
        public string Classification { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Date et heure de l’enregistrement de la mesure.")]
        public DateTimeOffset CreationDate { get; set; }

        [SwaggerSchema(Description = "Numéro d’opération ou étape de la gamme de test.")]
        public int OperationNumber { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’unité de mesure (ex: V, °C, Ohm).")]
        public string UnitUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant effectué la mesure.")]
        public string UserUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de la machine sur laquelle la mesure a été prise.")]
        public string MachineUid { get; set; } = string.Empty;
    }
}
