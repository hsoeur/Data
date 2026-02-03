using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    [SwaggerSchema(Description = "Utilisateur enregistré dans l’environnement de production. Utilisé pour la traçabilité des actions et des saisies opérateur.")]
    public class User
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l'utilisateur (clé primaire).")]
        public required string UserUid { get; set; }

        [SwaggerSchema(Description = "Nom de famille de l'utilisateur.")]
        public required string LastName { get; set; }

        [SwaggerSchema(Description = "Prénom de l'utilisateur.")]
        public required string FirstName { get; set; }

        [SwaggerSchema(Description = "Date d'entrée dans l'entreprise ou d'enregistrement dans le système.")]
        public required DateTimeOffset EntryDate { get; set; }

        [SwaggerSchema(Description = "Date de sortie ou de fin d'activité dans le système.")]
        public required DateTimeOffset ExitDate { get; set; }

        [SwaggerSchema(Description = "Département ou service auquel l'utilisateur est rattaché.")]
        public required string Department { get; set; }
    }
}
