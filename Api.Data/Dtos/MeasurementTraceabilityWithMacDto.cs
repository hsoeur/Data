namespace Api.Data.Dtos
{
    /// <summary>
    /// DTO pour créer une traçabilité de mesure en utilisant une adresse MAC
    /// </summary>
    public class MeasurementTraceabilityWithMacDto
    {
        public string ProductUid { get; set; } = string.Empty;
 public int SerialNumber { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public DateTimeOffset CreationDate { get; set; }
        public int OperationNumber { get; set; }
        public string UnitUid { get; set; } = string.Empty;
        public string UserUid { get; set; } = string.Empty;
        public string MachineUid { get; set; } = string.Empty;
   public int? ManufacturingOrderUid { get; set; }
   
        /// <summary>
        /// Si true, utilise la prochaine adresse MAC disponible. Si false, utilise MacAddressId.
        /// </summary>
  public bool UseNextAvailableMac { get; set; } = true;
        
        /// <summary>
        /// ID de l'adresse MAC spécifique à utiliser (optionnel si UseNextAvailableMac = true)
   /// </summary>
   public int? MacAddressId { get; set; }
    }
}
