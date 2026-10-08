namespace GenDb.Services;

public sealed class ImportServiceOptions
{
    public const string SectionName = "ImportService";

    public int PollIntervalSeconds { get; set; } = 60;
    public int StabilitySeconds { get; set; } = 60;
    public int MaximumFileAgeHours { get; set; } = 12;
    public int MaximumTimestampSpreadMinutes { get; set; } = 30;
    public int MinimumRowsPerFile { get; set; } = 1;
    public double MinimumRowCountRatio { get; set; } = 0.5;
    public string ProcessingRoot { get; set; } = @"C:\Data\GenDb\Processing";
    public string StateFile { get; set; } = @"C:\Data\GenDb\last-successful-batch.json";
    public bool KeepSuccessfulSnapshots { get; set; }
}
