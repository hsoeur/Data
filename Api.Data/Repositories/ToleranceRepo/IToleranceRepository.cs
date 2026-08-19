using DataLib;

using Api.Data.Dtos;

namespace Api.Data.Repositories.ToleranceRepo
{
    // Interface du dépôt pour Tolerance (vide pour l'instant)
    public interface IToleranceRepository
    {
        Task CreateTolerance(Tolerance tolerance);
        Task<List<Tolerance>> GetCollectionTolerances();
        Task<List<Tolerance>> GetCollectionTolerancesByProductUidByOperationByMachineLatestOne(string productUid, int operationNumber, string machine);
        Task<Tolerance?> UpdateTolerance(string toleranceUid, Tolerance tolerance);
        Task<List<Tolerance>> GetActiveTolerances();
        Task<CopyToleranceBatchResponse> CopyBatchAsync(
            CopyToleranceBatchRequest request,
            CancellationToken cancellationToken);
    }
}

