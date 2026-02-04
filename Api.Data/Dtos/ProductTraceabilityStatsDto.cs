namespace Api.Data.Dtos
{
    public class ProductTraceabilityStatsDto
    {
        public int OK { get; set; } = new();
        public int NOK { get; set; } = new();
        public double PercentageOK { get; set; }
        public int Total { get; set; }
    }
}
