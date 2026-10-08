using Microsoft.Extensions.Configuration;

namespace GenDb.Services;

public sealed record ExpectedExport(string Key, string SourcePath, int MinimumColumns);

public sealed record ExportFileState(
    string Key,
    string SourcePath,
    string SnapshotPath,
    long Length,
    DateTime LastWriteTimeUtc,
    string Sha256,
    int RowCount);

public sealed record SuccessfulBatchState(
    string BatchId,
    DateTime ImportedAtUtc,
    IReadOnlyList<ExportFileState> Files);

public sealed record PreparedBatch(
    string BatchId,
    string SnapshotDirectory,
    IReadOnlyList<ExportFileState> Files,
    IConfiguration SnapshotConfiguration);
