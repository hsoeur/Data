using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    /// <summary>
    /// Représente une tolérance de mesure dans la table `tolerance` de la base de production.
    /// Définie pour un produit, une opération, et une machine donnée.
    /// Utilisé par les services : Editeur et séquenceur de test.
    /// </summary>
    [SwaggerSchema(Description = "Tolérance définie pour un produit, une opération et une machine. Encadre les valeurs acceptables pour les mesures de production.")]
    public class Tolerance
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la tolérance (clé primaire).")]
        public required string ToleranceUid { get; set; }

        [SwaggerSchema(Description = "Identifiant du produit concerné par la tolérance.")]
        [ForeignKey(nameof(Article))]
        public required string ProductUid { get; set; }
        public Article? Article { get; set; }

        [SwaggerSchema(Description = "Nom ou libellé de la caractéristique mesurée (ex : Tension, Température, etc.).")]
        public required string Label { get; set; }

        [SwaggerSchema(Description = "Valeur minimale ou seuil bas de la tolérance.")]
        public required string Val1 { get; set; }

        [SwaggerSchema(Description = "Valeur maximale ou seuil haut de la tolérance.")]
        public required string Val2 { get; set; }

        [SwaggerSchema(Description = "Date de début de validité de la tolérance.")]
        public required DateTimeOffset StartDate { get; set; }

        [SwaggerSchema(Description = "Date de fin de validité de la tolérance.")]
        public required DateTimeOffset EndDate { get; set; }

        [SwaggerSchema(Description = "Numéro d’opération dans la gamme de fabrication.")]
        public required int OperationNumber { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant défini ou modifié la tolérance.")]
        public required string UserUid { get; set; } = string.Empty;

        [ForeignKey(nameof(UserUid))]
        [SwaggerSchema(Description = "Objet représentant l’utilisateur associé à la tolérance.")]
        public User? User { get; set; }

        [SwaggerSchema(Description = "Identifiant de la machine concernée.")]
        public required string MachineUid { get; set; } = string.Empty;

        [ForeignKey(nameof(MachineUid))]
        [SwaggerSchema(Description = "Objet représentant la machine sur laquelle s’applique la tolérance.")]
        public Machine? Machine { get; set; }

        [SwaggerSchema(Description = "Mode de mesure concerné.")]
        public required Mode Mode { get; set; }

        [SwaggerSchema(Description = "Unité utilisée pour la mesure.")]
        public required Unit Unit { get; set; }
    }
}
