using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    /// <summary>
    /// Représente un template d’étiquette dans la table `template` de la base de production.
    /// Chaque template est associé à un produit et un usage spécifiques.
    /// Utilisé par les services : Impression des étiquettes, gestion des modèles.
    /// </summary>
    [SwaggerSchema(Description = "Template d’étiquette utilisé pour l’impression. Associé à un produit et un usage. Contient les fichiers du template et l’image de prévisualisation.")]
    public class LabelTemplate
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique du template (clé primaire).")]
        public required string LabelTemplateUid { get; set; }

        [SwaggerSchema(Description = "Code produit auquel est associé le template.")]
        [ForeignKey(nameof(Article))]
        public required string Product { get; set; }

        [SwaggerSchema(Description = "Article référencé par le code produit du template.")]
        public Article? Article { get; set; }

        [SwaggerSchema(Description = "Usage de l’étiquette (ex : Production, SAV, Test...).")]
        [ForeignKey(nameof(LabelType))]
        public required string LabelTypeUid { get; set; }

        [SwaggerSchema(Description = "Type d'étiquette référencé par l'identifiant de type.")]
        public LabelType? LabelType { get; set; }

        [SwaggerSchema(Description = "Nom ou chemin du fichier de template utilisé pour l’impression.")]
        public required string TemplateFile { get; set; }

        [SwaggerSchema(Description = "Nom ou chemin de l’image associée au template pour visualisation.")]
        public required string Photo { get; set; }

        [SwaggerSchema(Description = "Date de début de validité du template.")]
        public required DateTimeOffset StartDate { get; set; }

        [SwaggerSchema(Description = "Date de fin de validité du template.")]
        public required DateTimeOffset EndDate { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant créé ou modifié le template.")]
        public required string UserUid { get; set; }
    }
}
