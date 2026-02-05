using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
    public class Firmware
    {
        [Key]
        [SwaggerSchema(Description = "Identifiant unique du firmware (clé primaire).")]
        public required string FirmwareUid { get; set; }

        [SwaggerSchema(Description = "Code du produit associé à ce firmware.")]
        [ForeignKey(nameof(Article))]
        public required string Product { get; set; }

        // Navigation vers l'article (clé étrangère sur Product -> Article.ArticleUid)
        [SwaggerSchema(Description = "Article associé au firmware (référence).")]
        public Article? Article { get; set; }

        [SwaggerSchema(Description = "Libellé ou nom de l’usage du firmware.")]
        public required string Label { get; set; }

        [SwaggerSchema(Description = "Nom du fichier firmware stocké sur le serveur.")]
        public required string File { get; set; }

        [SwaggerSchema(Description = "Version logicielle du firmware.")]
        public required string Version { get; set; }

        [SwaggerSchema(Description = "Identifiant de l’utilisateur ayant enregistré ou modifié le firmware.")]
        [ForeignKey(nameof(User))]
        public required string UserUid { get; set; }

        // Navigation vers l'utilisateur (clé étrangère sur UserUid -> User.UserUid)
        [SwaggerSchema(Description = "Utilisateur associé au firmware (référence).")]
        public User? User { get; set; }

        [SwaggerSchema(Description = "Date de début de validité du firmware.")]
        public required DateTimeOffset StartDate { get; set; }

        [SwaggerSchema(Description = "Date de fin de validité du firmware.")]
        public required DateTimeOffset EndDate { get; set; }

        [SwaggerSchema(Description = "Indique si le firmware est suspendu (O/N).")]
        public required bool IsSuspended { get; set; }

        public override string ToString()
        {
            return $"Firmware: " +
                   $"Uid = {FirmwareUid}, " +
                   $"Product = {Product}, " +
                   $"Label = {Label}, " +
                   $"File = {File}, " +
                   $"Version = {Version}, " +
                   $"UserUid = {UserUid}, " +
                   $"StartDate = {StartDate}, " +
                   $"EndDate = {EndDate}";
        }
    }
}
