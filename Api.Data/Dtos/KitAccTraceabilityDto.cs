using Swashbuckle.AspNetCore.Annotations;
using System;

namespace Api.Data.Dtos
{
    /// <summary>
    /// Données de création/transport pour la traçabilité d'un accessoire de kit.
    /// </summary>
    [SwaggerSchema(Description = "DTO pour la traçabilité des accessoires de kit, utilisé pour les opérations API (création, consultation).")]
    public class KitAccTraceabilityDto
    {


        [SwaggerSchema(Description = "Numéro de commande client associé à l’accessoire.")]
        public string ClientOrder { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de la ligne de commande à laquelle est rattaché l’accessoire.")]
        public string OrderLineUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Code produit de l’accessoire.")]
        public string Product { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de série de l’accessoire.")]
        public int SerialNumber { get; set; }

        [SwaggerSchema(Description = "Numéro de scan (frame) utilisé lors du kitting.")]
        public string ScanFrame { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant réalisé le kitting.")]
        public string UserUid { get; set; } = string.Empty;
    }
}
