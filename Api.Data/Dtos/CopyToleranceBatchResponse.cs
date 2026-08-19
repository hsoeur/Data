namespace Api.Data.Dtos;

public sealed record CopyToleranceBatchResponse(
    string SourceProductUid,
    string TargetProductUid,
    int CreatedCount,
    IReadOnlyCollection<ToleranceDto> Tolerances);
