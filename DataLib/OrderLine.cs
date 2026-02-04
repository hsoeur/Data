using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    public class OrderLine
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la ligne de commande (clé primaire).")]
        public required string UidOrderLine { get; set; }

        [SwaggerSchema(Description = "Code produit associé à cette ligne de commande.")]
        public required string Product { get; set; }

        [SwaggerSchema(Description = "Quantité à produire ou à préparer pour cette ligne.")]
        public int Quantity { get; set; }
    }
}
