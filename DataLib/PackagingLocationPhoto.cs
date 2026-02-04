using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Représente une photo d’article dans la table `photoarticle` de la base de production.
    /// Données utilisées par le service de visualisation des emplacements pour les bornes.
    /// Permet d’associer une image à un article pour un cas d’usage spécifique.
    /// </summary>
    [SwaggerSchema(Description = "Photo associée à un article pour visualisation sur les bornes. Utilisée pour aider à l’identification ou à l’emplacement du composant.")]
    public class PackagingLocationPhoto
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la photo d’article (clé primaire).")]
        public required string PackagingLocationPhotoUid { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’article associé à la photo.")]
        public required string ArticleUid { get; set; }

        [SwaggerSchema(Description = "Chemin ou nom du fichier image représentant l’article.")]
        public required string Photo { get; set; }

        [SwaggerSchema(Description = "Cas d’usage de la photo (ex : Emplacement, Contrôle, Assemblage, etc.).")]
        public required string UseCase { get; set; }
    }
}
