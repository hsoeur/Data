using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Dtos
{
 /// <summary>
 /// Données de création/transport pour la traçabilité d'un kit.
 /// </summary>
 [SwaggerSchema(Description = "DTO pour la traçabilité des kits, utilisé pour les opérations API (création, transfert).")]
 public class KitTraceabilityDto
 {
 [SwaggerSchema(Description = "Identifiant unique du kit (généré côté serveur).")]
 public string? KitTraceabilityUid { get; set; }

 [SwaggerSchema(Description = "Numéro de commande client associé au kit.")]
 public string ClientOrder { get; set; } = string.Empty;

 [SwaggerSchema(Description = "Identifiant de l’ordre de fabrication lié au kit.")]
 public int ManufacturingOrderUid { get; set; }

 [SwaggerSchema(Description = "Code produit du kit.")]
 public string Product { get; set; } = string.Empty;

 [SwaggerSchema(Description = "Code interne du composant Horizon utilisé.")]
 public string HorizonComponentCode { get; set; } = string.Empty;

 [SwaggerSchema(Description = "Numéro de scan (frame) utilisé pour l’identification du kit.")]
 public string ScanFrame { get; set; } = string.Empty;

 [SwaggerSchema(Description = "Date de création du kit.")]
 public DateTimeOffset Date { get; set; }

 [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant créé le kit.")]
 public string UserUid { get; set; } = string.Empty;
 }
}
