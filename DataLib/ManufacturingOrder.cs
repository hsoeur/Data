using System;
using System.ComponentModel.DataAnnotations;

namespace DataLib
{
    /// <summary>
    /// Immutable domain entity for manufacturing order.
    /// Prefer using DTOs in API layer for transport concerns.
    /// </summary>
    public record class ManufacturingOrder
    {
        /// <summary>
        /// Unique identifier (primary key).
        /// </summary>
        [Key]
        public int ManufacturingOrderUid { get; init; }

        /// <summary>
        /// Product unique identifier.
        /// </summary>
        [Required]
        public string ProductUid { get; init; } = string.Empty;

        /// <summary>
        /// Total quantity to produce.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Quantity { get; init; }

        /// <summary>
        /// First serial number assigned to this order.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int FirstSerialNumber { get; init; } 

        /// <summary>
        /// Internal order number for production management.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string InternalOrderNumber { get; init; } = string.Empty;

        /// <summary>
        /// External order number (client or ERP/GPAO).
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ExternalOrderNumber { get; init; } = string.Empty;

        /// <summary>
        /// Status in production workflow.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Status { get; set; } = string.Empty;

    }
}
