using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DataLib
{
    public class Operation
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l’opération (clé primaire).")]
        public string OperationUid { get; set; }

        [SwaggerSchema(Description = "Identifiant du produit associé à l’opération.")]
        public string ProductUid { get; set; }

        [SwaggerSchema(Description = "Numéro de l’opération dans la gamme de fabrication.")]
        public int OperationNumber { get; set; }

        [SwaggerSchema(Description = "Libellé ou nom de l’opération (ex : Test final, Assemblage, etc.).")]
        public string Label { get; set; }

        [SwaggerSchema(Description = "Nom ou identifiant de la machine affectée à l’opération.")]
        public string Machine { get; set; }

        [SwaggerSchema(Description = "Temps de préparation estimé pour cette opération (en minutes, 2 décimales).")]
        [Precision(10,2)]
        public decimal PrepTime { get; set; }

        [SwaggerSchema(Description = "Temps de production estimé pour cette opération (en minutes, 2 décimales).")]
        [Precision(10,2)]
        public decimal ProdTime { get; set; }

        [SwaggerSchema(Description = "Date de création ou d’import de l’opération.")]
        public DateTimeOffset CreationDate { get; set; }
    }
}
