using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Notification de base de données représentant une modification sur une entité.
    /// </summary>
    [SwaggerSchema(Description = "Notification de modification d'une entité en base de données.")]
    public class DatabaseNotification
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la notification (clé primaire).")]
        public string DatabaseNotificationUid { get; set; } // Clé primaire

        [SwaggerSchema(Description = "Type de l'entité concernée (ex: OF, Produit, Article).")]
        public string EntityType { get; set; } // ex: "OF", "Produit", "Article"

        [SwaggerSchema(Description = "Identifiant unique de l'entité concernée.")]
        public string EntityUId { get; set; } // ex: "123", "SKU-456"

        [SwaggerSchema(Description = "Nom de la propriété modifiée.")]
        public string PropertyName { get; set; } // ex: "Quantity", "SerialNumber"

        [SwaggerSchema(Description = "Ancienne valeur de la propriété.")]
        public string OldValue { get; set; }

        [SwaggerSchema(Description = "Nouvelle valeur de la propriété.")]
        public string NewValue { get; set; }

        [SwaggerSchema(Description = "Date de modification (texte).")]
        public DateTimeOffset ModifiedAt { get; set; }

        [SwaggerSchema(Description = "Utilisateur ayant réalisé la modification.")]
        public string ModifiedBy { get; set; }

        [SwaggerSchema(Description = "Indique si la notification a été traitée (texte).")]
        public string Processed { get; set; }

        [SwaggerSchema(Description = "Date de traitement (texte).")]
        public DateTimeOffset ProcessedDate { get; set; }

        [SwaggerSchema(Description = "Utilisateur ayant traité la notification.")]
        public string ProcessedBy { get; set; }

        [SwaggerSchema(Description = "Commentaire complémentaire.")]
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"DatabaseNotification [DatabaseNotificationUid={DatabaseNotificationUid}, EntityType={EntityType}, EntityUId={EntityUId}, PropertyName={PropertyName}, OldValue={OldValue}, NewValue={NewValue}, ModifiedAt={ModifiedAt}, ModifiedBy={ModifiedBy}, Processed={Processed}, ProcessedDate={ProcessedDate}, ProcessedBy={ProcessedBy}, Comment={Comment}]";
        }
    }
}
