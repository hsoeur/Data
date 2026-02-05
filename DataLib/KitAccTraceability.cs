using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Accessoire rattaché à un kit dans le cadre d'une commande client. Permet la traçabilité des produits complémentaires avec numéro de série.
    /// </summary>
    [SwaggerSchema(Description = "Accessoire rattaché à un kit dans le cadre d'une commande client. Permet la traçabilité des produits complémentaires avec numéro de série.")]
    public class KitAccTraceability
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l'accessoire de kit (clé primaire).")]
        public required string KitAccTraceabilityUid { get; set; }

        [SwaggerSchema(Description = "Numéro de commande client associé à l’accessoire.")]
        public required string ClientOrder { get; set; }

        [SwaggerSchema(Description = "Identifiant de la ligne de commande à laquelle est rattaché l’accessoire.")]
        public required string OrderLineUid { get; set; }

        [SwaggerSchema(Description = "Code produit de l’accessoire.")]
        public required string Product { get; set; }

        [SwaggerSchema(Description = "Numéro de série de l’accessoire.")]
        public required int SerialNumber { get; set; }

        [SwaggerSchema(Description = "Numéro de scan (frame) utilisé lors du kitting.")]
        public required string ScanFrame { get; set; }

        [SwaggerSchema(Description = "Date d’enregistrement de l’accessoire.")]
        public required DateTimeOffset Date { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant réalisé le kitting.")]
        public required string UserUid { get; set; }
    }
}