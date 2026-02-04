using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Statut d'un produit dans la traçabilité (ex: OK, NOK, En cours).")]
    public class ProductStatus
    {
        [Key]
        public required string ProductStatusUid { get; set; }

        // Liste des produits qui utilisent ce statut
        public List<ProductTraceability>? ProductTraceabilities { get; set; }
    }
}
