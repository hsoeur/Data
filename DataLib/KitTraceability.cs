using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Représente un kit de production dans la table `kit` de la base de données de production.
    /// Données générées lors du kitting des bornes.
    /// Utilisé par les services : Kitting des bornes.
    /// </summary>
    [SwaggerSchema(Description = "Kit de composants préparé pour un ordre de fabrication donné. Contient les informations de traçabilité, de scan et de création.")]
    public class KitTraceability
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique du kit (clé primaire).")]
        public string KitTraceabilityUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de commande client associé au kit.")]
        public string ClientOrder { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant de l’ordre de fabrication lié au kit.")]
        public int ManufacturingOrderUid { get; set; }

        [SwaggerSchema(Description = "Code produit du kit.")]
        public string Product { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Code interne du composant Horizon utilisé.")]
        public string HorizonComponentCode { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de série.")]
        public int SerialNumber { get; set; }

        [SwaggerSchema(Description = "Numéro de scan (frame) utilisé pour l’identification du kit.")]
        public string ScanFrame { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Date de création du kit.")]
        public DateTimeOffset Date { get; set; } 

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant créé le kit.")]
        public string UserUid { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"KitTraceability [KitUid={KitTraceabilityUid}, ClientOrder={ClientOrder}, ManufacturingOrderUid={ManufacturingOrderUid}, " +
            $"Product={Product}, HorizonComponentCode={HorizonComponentCode}, ScanFrame={ScanFrame}, " +
            $"Date={Date}, UserUid={UserUid}]";
        }
    }
}
