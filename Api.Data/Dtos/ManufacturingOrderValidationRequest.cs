using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Dtos
{
    public sealed class ManufacturingOrderValidationRequest
    {
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Le suffixe produit doit contenir exactement 6 caractères.")]
        [SwaggerSchema(Description = "Six derniers caractères de la référence produit. Un espace final est significatif.", Nullable = false)]
        public string ProductSuffix { get; init; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Le numéro de série doit contenir exactement 6 chiffres.")]
        [SwaggerSchema(Description = "Numéro de série sur exactement 6 chiffres.", Nullable = false)]
        public string SerialNumber { get; init; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "L'identifiant de l'OF doit contenir exactement 5 chiffres.")]
        [SwaggerSchema(Description = "Identifiant de l'ordre de fabrication sur exactement 5 chiffres.", Nullable = false)]
        public string ManufacturingOrderId { get; init; } = string.Empty;
    }
}
