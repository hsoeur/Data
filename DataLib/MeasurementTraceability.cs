using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Mesure de production rattachee a une trace produit et a une tolerance.")]
    public class MeasurementTraceability
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la trace de mesure.")]
        public string MeasurementTraceabilityUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de la trace produit associee.")]
        public string ProductTraceabilityUid { get; set; } = string.Empty;

        [ForeignKey(nameof(ProductTraceabilityUid))]
        [SwaggerSchema(Description = "Trace produit associee a cette mesure.")]
        public ProductTraceability? ProductTraceability { get; set; }

        [SwaggerSchema(Description = "Identifiant de la tolerance utilisee pour classifier la mesure.")]
        public string ToleranceUid { get; set; } = string.Empty;

        [ForeignKey(nameof(ToleranceUid))]
        [SwaggerSchema(Description = "Tolerance associee a cette mesure.")]
        public Tolerance? Tolerance { get; set; }

        [MaxLength(128)]
        [SwaggerSchema(Description = "Valeur mesuree.")]
        public string Value { get; set; } = string.Empty;

        [MaxLength(32)]
        [SwaggerSchema(Description = "Classification de la mesure.")]
        public string Classification { get; set; } = string.Empty;
    }
}
