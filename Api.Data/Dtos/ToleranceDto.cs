using DataLib;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Api.Data.Dtos
{
    public class ToleranceDto
    {
        public required string ProductUid { get; set; }
        public required int OperationNumber { get; set; }
        public required string MachineUid { get; set; }
        public required string Label { get; set; }
        [ValidMode]
        public required string Mode { get; set; }
        public required string Val1 { get; set; }
        public required string Val2 { get; set; }
        [SwaggerSchema(Description = "Unité utilisée pour la mesure.")]
        public required string UnitUid { get; set; }
        public required string UserUid { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
    }
}
