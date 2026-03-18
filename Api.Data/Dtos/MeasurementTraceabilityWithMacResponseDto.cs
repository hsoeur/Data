using DataLib;

namespace Api.Data.Dtos
{
    /// <summary>
    /// DTO de réponse pour la création d'une traçabilité de mesure avec adresse MAC
 /// </summary>
    public class MeasurementTraceabilityWithMacResponseDto
    {
        /// <summary>
        /// La traçabilité de mesure créée
        /// </summary>
  public MeasurementTraceability MeasurementTraceability { get; set; } = null!;
        
        /// <summary>
        /// L'adresse MAC utilisée et marquée comme USED
      /// </summary>
    public MacAddress UsedMacAddress { get; set; } = null!;
    }
}
