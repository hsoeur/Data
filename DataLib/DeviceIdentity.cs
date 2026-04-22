using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    /// <summary>
    /// Represents the association between a device equipment serial number and a MAC address
    /// </summary>
    [SwaggerSchema(Description = "Association entre un numéro de série d'équipement (EquipSN) et une adresse MAC. Chaque EquipSN et chaque MAC est unique dans la table.")]
    [Table("DeviceIdentities")]
    public class DeviceIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SwaggerSchema(Description = "Identifiant unique de l'association appareil (clé primaire).")]
        public int Id { get; set; }

        /// <summary>
        /// Equipment Serial Number (EquipSN) - Must be unique
        /// Example: 00634A
        /// </summary>
        [Required]
        [StringLength(50)]
        [SwaggerSchema(Description = "Numéro de série de l'équipement (EquipSN). Doit être unique. Exemple: 00634A")]
        public string EquipSN { get; set; } = string.Empty;

        /// <summary>
        /// MAC Address Value - Must be unique
        /// Format: 00:1A:2B:00:00:01
        /// </summary>
        [Required]
        [Column(TypeName = "char(17)")]
        [StringLength(17)]
        [SwaggerSchema(Description = "Adresse MAC associée à l'équipement. Doit être unique. Format: 00:1A:2B:00:00:01")]
        public string MacAddressValue { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the device identity was created
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "Date et heure de création de l'association.")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

       
    }
}
