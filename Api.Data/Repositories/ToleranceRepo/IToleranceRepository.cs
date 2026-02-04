using DataLib;

namespace Api.Data.Repositories.ToleranceRepo
{
    // Interface du dépôt pour Tolerance (vide pour l'instant)
    public interface IToleranceRepository
    {
        Task CreateTolerance(Tolerance tolerance);
        Task<List<Tolerance>> GetCollectionTolerances();
        Task<List<Tolerance>> GetCollectionTolerancesByProductUidByOperationByMachineLatestOne(string productUid, int operationNumber, string machine);
    }
}
