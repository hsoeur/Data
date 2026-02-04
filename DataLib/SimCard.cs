using System;
using System.ComponentModel.DataAnnotations;

namespace DataLib
{
    public class SimCard
    {
        [Key]
        public string SimCardUid { get; set; } = string.Empty;

        [Phone]
        [MaxLength(32)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string Provider { get; set; } = string.Empty;

        // ICCID is typically a long numeric string with leading zeros; keep as string
        [Required]
        [MaxLength(32)]
        public string SimCardNumber { get; set; } = string.Empty;
    }
}
