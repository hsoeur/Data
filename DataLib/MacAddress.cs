using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib
{
    /// <summary>
    /// Represents a MAC address with its availability status
    /// </summary>
    [Table("MacAddresses")]
    public class MacAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// MAC Address in format: 00:1A:2B:00:00:01
        /// </summary>
        [Required]
        [Column(TypeName = "char(17)")]
        [StringLength(17)]
        public string MacAddressValue { get; set; } = string.Empty;

        /// <summary>
        /// Status of the MAC address: AVAILABLE, RESERVED, or USED
        /// </summary>
        [Required]
        [Column(TypeName = "enum('AVAILABLE','RESERVED','USED')")]
        public MacAddressStatus Status { get; set; } = MacAddressStatus.AVAILABLE;

        /// <summary>
        /// Timestamp when the MAC address was reserved
        /// </summary>
        public DateTime? ReservedAt { get; set; }

        /// <summary>
        /// Timestamp when the MAC address was used
        /// </summary>
        public DateTime? UsedAt { get; set; }

        /// <summary>
        /// Timestamp when the MAC address was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Enum for MAC address status
    /// </summary>
    public enum MacAddressStatus
    {
        AVAILABLE,
        RESERVED,
        USED
    }
}
