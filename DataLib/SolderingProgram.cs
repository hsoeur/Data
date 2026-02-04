using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DataLib
{
 /// <summary>
 /// Représente un programme de soudure dans la table `solderingprogram` de la base de production.
 /// Données utilisées pour définir quel programme appliquer à un produit selon la face à souder.
 /// Utilisé par les services : Gestion des programmes de brasage avec le prodmanager et service de programmation des tags.
 /// </summary>
 [SwaggerSchema(Description = "Programme de soudure associé à un produit et une face spécifique (TOP/BOTTOM). Permet de piloter les machines de soudure.")]
 public class SolderingProgram
 {
 [Key]
 [SwaggerSchema(Description = "Identifiant unique du programme de soudure (clé primaire).")]
 public required string SolderingProgramUid { get; set; }

 [SwaggerSchema(Description = "Identifiant du produit auquel est associé le programme.")]
 public required string ProductUid { get; set; }

 [SwaggerSchema(Description = "Nom ou référence du programme de soudure.")]
 public required string Program { get; set; }

 [SwaggerSchema(Description = "Date de début de validité du programme.")]
 public required DateTimeOffset StartDate { get; set; }

 [SwaggerSchema(Description = "Date de fin de validité du programme.")]
 public required DateTimeOffset EndDate { get; set; }

 [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant créé ou modifié le programme.")]
 public required string UserUid { get; set; }

 [SwaggerSchema(Description = "Côté du produit concerné par le programme : 'FS' ou 'FC'.")]
 public required string Side { get; set; }
 }
}
