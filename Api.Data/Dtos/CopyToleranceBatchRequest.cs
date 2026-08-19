using System.ComponentModel.DataAnnotations;

namespace Api.Data.Dtos;

public sealed record CopyToleranceBatchRequest(
    string SourceProductUid,
    string TargetProductUid,
    IReadOnlyCollection<string> ToleranceUids,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string UserUid);
