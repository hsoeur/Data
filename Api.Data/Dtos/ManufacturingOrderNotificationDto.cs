using System;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Data.Dtos
{
    public class ManufacturingOrderNotificationDto
    {
        [SwaggerSchema(Description = "Identifiant unique de la notification (optionnel lors de la création).")]
        public string NotificationUid { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Numéro de l’ordre de fabrication concerné.")]
        public int OfNumber { get; set; }

        [SwaggerSchema(Description = "Type de changement détecté : 'Quantité' ou 'Numéro de série'.")]
        public string ChangeType { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Ancienne valeur détectée (base de production).")]
        public int OldValue { get; set; }

        [SwaggerSchema(Description = "Nouvelle valeur détectée (base GPAO).")]
        public int NewValue { get; set; } 

        [SwaggerSchema(Description = "Date de création de la notification.")]
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
