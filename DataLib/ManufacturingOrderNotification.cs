using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataLib
{
    public class ManufacturingOrderNotification
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la notification (clé primaire).")]
        public string NotificationUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de l’ordre de fabrication concerné.")]
        public int OfNumber { get; set; }

        [SwaggerSchema(Description = "Type de changement détecté : 'Quantité' ou 'Numéro de série'.")]
        public string ChangeType { get; set; }

        [SwaggerSchema(Description = "Ancienne valeur détectée (base de production).")]
        public int OldValue { get; set; }

        [SwaggerSchema(Description = "Nouvelle valeur détectée (base GPAO).")]
        public int NewValue { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant traité la notification.")]
        public string ProcessedBy { get; set; }

        [SwaggerSchema(Description = "Date de création de la notification."), DataType(DataType.DateTime)]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        [SwaggerSchema(Description = "Statut de traitement de la notification (O/N).")]
        public string Processed { get; set; }

        [SwaggerSchema(Description = "Date à laquelle la notification a été traitée."), DataType(DataType.DateTime)]
        public DateTimeOffset? ProcessedDate { get; set; }
    }
}
