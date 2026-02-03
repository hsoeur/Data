using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class Article
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de l'article (clé primaire).")]
        public required string ArticleUid { get; set; }

        [SwaggerSchema(Description = "Désignation commerciale de l'article.")]
        public required string Designation { get; set; }

        [SwaggerSchema(Description = "Nom de l'emplacement de stockage de l'article.")]
        public required string Store { get; set; }

        [SwaggerSchema(Description = "Famille ou catégorie logistique de l'article.")]
        public required string Family { get; set; }

        [SwaggerSchema(Description = "Type d'emballage associé à l'article.")]
        public required string Packaging { get; set; }

        [SwaggerSchema(Description = "Valeur booléenne (O/N) indiquant si la gestion de stock est activée pour l'article.")]
        public required string ManageStock { get; set; }

        [SwaggerSchema(Description = "Référence du fournisseur telle que renseignée dans la GPAO.")]
        public required string SupplierReference { get; set; }

        [SwaggerSchema(Description = "Quantité GPAO attendue pour cet article (souvent liée aux ordres de fabrication).")]
        public required int GpaoQuantity { get; set; }
    }
}
