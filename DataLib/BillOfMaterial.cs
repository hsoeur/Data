using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class BillOfMaterial
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique de la nomenclature (clé primaire).")]
        public required string BillOfMaterialsUid { get; set; }

        [SwaggerSchema(Description = "Code du produit final auquel cette nomenclature appartient.")]
        public required string Product { get; set; }

        [SwaggerSchema(Description = "Code du composant utilisé dans la nomenclature.")]
        public required string Component { get; set; }

        [SwaggerSchema(Description = "Code de l'opération ou de la gamme de production associée.")]
        public required string Operation { get; set; }

        [SwaggerSchema(Description = "Quantité brute requise du composant.")]
        public required string Quantity { get; set; }

        [SwaggerSchema(Description = "Quantité divisée ou ajustée du composant (souvent pour les implantations multiples).")]
        public required string QuantityDiv { get; set; }

        [SwaggerSchema(Description = "Date de création ou d'entrée en vigueur de la nomenclature.")]
        public required DateTimeOffset CreationDate { get; set; }

        [SwaggerSchema(Description = "Date de fin de validité de la nomenclature.")]
        public required DateTimeOffset EndDate { get; set; }
    }
}
