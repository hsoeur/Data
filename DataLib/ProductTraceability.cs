using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    [SwaggerSchema(Description = "Traçabilité d’un produit à numéro de série unique, associée à une opération, une machine, un utilisateur et un statut.")]
    public class ProductTraceability
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la trace produit (clé primaire).")]
        public string ProductTraceabilityUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Identifiant du produit concerné.")]
        public string ProductUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de série unique du produit.")]
        public int ProductSerialNumber { get; set; } = 0;

        [SwaggerSchema(Description = "Identifiant du statut associé à cette étape de production.")]
        public string ProductStatusUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Commentaire décrivant la panne lorsque le produit est NOK ou KO.")]
        public string? FailureComment { get; set; }

        [ForeignKey(nameof(ProductStatusUid))]
        [SwaggerSchema(Description = "Objet représentant le statut du produit (clé étrangère).")]
        public ProductStatus? ProductStatus { get; set; }

        [SwaggerSchema(Description = "Date et heure de l’enregistrement de la trace.")]
        public DateTimeOffset CreationDate { get; set; } 

        [SwaggerSchema(Description = "Identifiant de l’ordre de fabrication associé.")]
        public int ManufacturingOrderUid { get; set; } 

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant effectué l’opération.")]
        public string UserUid { get; set; } = string.Empty;

        [ForeignKey(nameof(UserUid))]
        [SwaggerSchema(Description = "Objet représentant l’utilisateur (clé étrangère).")]
        public User? User { get; set; }

        [SwaggerSchema(Description = "Temps de cycle mesuré pour l’opération (en secondes ou format temps).")]
        [Precision(10, 2)]
        public decimal CycleTime { get; set; } 

        [SwaggerSchema(Description = "Numéro de l’opération dans la gamme de production.")]
        public int OperationNumber { get; set; }

        [SwaggerSchema(Description = "Identifiant de la machine utilisée.")]
        public string MachineUid { get; set; } = string.Empty;

        [ForeignKey(nameof(MachineUid))]
        [SwaggerSchema(Description = "Objet représentant la machine utilisée (clé étrangère).")]
        public Machine? Machine { get; set; }
    }
}
