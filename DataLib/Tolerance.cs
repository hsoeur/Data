using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    /// <summary>
    /// Represente une tolerance de mesure dans la table `Tolerances`.
    /// Definie pour un produit, une operation et une machine donnee.
    /// </summary>
    [SwaggerSchema(Description = "Tolerance definie pour un produit, une operation et une machine. Encadre les valeurs acceptables pour les mesures de production.")]
    public class Tolerance
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la tolerance (cle primaire).")]
        public required string ToleranceUid { get; set; }

        [SwaggerSchema(Description = "Identifiant du produit concerne par la tolerance.")]
        [ForeignKey(nameof(Article))]
        public required string ProductUid { get; set; }
        public Article? Article { get; set; }

        [SwaggerSchema(Description = "Nom ou libelle de la caracteristique mesuree (ex : Tension, Temperature, etc.).")]
        public required string Label { get; set; }

        [SwaggerSchema(Description = "Valeur minimale ou seuil bas de la tolerance.")]
        public required string Val1 { get; set; }

        [SwaggerSchema(Description = "Valeur maximale ou seuil haut de la tolerance.")]
        public required string Val2 { get; set; }

        [SwaggerSchema(Description = "Date de debut de validite de la tolerance.")]
        public required DateTimeOffset StartDate { get; set; }

        [SwaggerSchema(Description = "Date de fin de validite de la tolerance.")]
        public required DateTimeOffset EndDate { get; set; }

        [SwaggerSchema(Description = "Numero d'operation dans la gamme de fabrication.")]
        public required int OperationNumber { get; set; }

        [SwaggerSchema(Description = "Identifiant de l'utilisateur ayant defini ou modifie la tolerance.")]
        public required string UserUid { get; set; } = string.Empty;

        [ForeignKey(nameof(UserUid))]
        [SwaggerSchema(Description = "Objet representant l'utilisateur associe a la tolerance.")]
        public User? User { get; set; }

        [SwaggerSchema(Description = "Identifiant de la machine concernee.")]
        public required string MachineUid { get; set; } = string.Empty;

        [ForeignKey(nameof(MachineUid))]
        [SwaggerSchema(Description = "Objet representant la machine sur laquelle s'applique la tolerance.")]
        public Machine? Machine { get; set; }

        [SwaggerSchema(Description = "Mode de mesure concerne.")]
        [ValidMode]
        public required string Mode { get; set; }

        [SwaggerSchema(Description = "Identifiant de l'unite utilisee pour la mesure.")]
        [ForeignKey(nameof(Unit))]
        public required string UnitUid { get; set; }
        public Unit? Unit { get; set; }
    }
}
