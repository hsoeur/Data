using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Représente une étiquette produit dans la table `label` de la base de production.
    /// Données générées lors de l’impression d’étiquettes via l’outil d'impression des étiquettes de production.
    /// Utilisé par les services : Impression d’étiquettes de production.
    /// </summary>
    [SwaggerSchema(Description = "Étiquette générée pour un produit en production. Contient les informations de traçabilité, version produit et usage.")]
    public class Label
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l'étiquette (clé primaire).")]
        public required string LabelUid { get; set; }

        [SwaggerSchema(Description = "Identifiant unique d'authentification lié à la session utilisateur ou au poste.")]
        public required string AuthentiUid { get; set; }

        [SwaggerSchema(Description = "Identifiant de l'utilisateur ayant généré l'étiquette.")]
        [ForeignKey(nameof(User))]
        public required string UserUid { get; set; }

        [SwaggerSchema(Description = "Utilisateur référencé par l'identifiant.")]
        public User? User { get; set; }

        [SwaggerSchema(Description = "Identifiant du produit concerné par l'étiquette.")]
        [ForeignKey(nameof(Article))]
        public required string ProductUid { get; set; }

        [SwaggerSchema(Description = "Article référencé par le code produit de l'étiquette.")]
        public Article? Article { get; set; }

        [SwaggerSchema(Description = "Identifiant de l'ordre de fabrication lié à l'étiquette.")]
        [ForeignKey(nameof(ManufacturingOrder))]
        public required int ManufacturingOrderUid { get; set; }

        [SwaggerSchema(Description = "Ordre de fabrication référencé par l'identifiant.")]
        public ManufacturingOrder? ManufacturingOrder { get; set; }

        [SwaggerSchema(Description = "Date de création de l'étiquette.")]
        public required DateTimeOffset CreationDate { get; set; }

        [SwaggerSchema(Description = "Numéro de série courant attribué au produit.")]
        public required int CurrentSerialNumber { get; set; }

        [SwaggerSchema(Description = "Version du produit liée à l'étiquette.")]
        public required string ProductVersion { get; set; }

        [SwaggerSchema(Description = "Usage associé à l’étiquette (ex : Test, Production, SAV...).")]
        [ForeignKey(nameof(LabelType))]
        public required string LabelTypeUid { get; set; }

        [SwaggerSchema(Description = "Type d'étiquette référencé par l'identifiant de type.")]
        public LabelType? LabelType { get; set; }

        [SwaggerSchema(Description = "Nom du template utilisé pour générer l’étiquette.")]
        public required string Template { get; set; }

        public override string ToString()
        {
            return $"Label [LabelUid={LabelUid}, AuthentiUid={AuthentiUid}, UserUid={UserUid}, ProductUid={ProductUid}, " +
            $"ManufacturingOrderUid={ManufacturingOrderUid}, CreationDate={CreationDate}, " +
            $"CurrentSerialNumber={CurrentSerialNumber}, ProductVersion={ProductVersion}, " +
            $"LabelTypeUid={LabelTypeUid}, Template={Template}]";
        }
    }
}
