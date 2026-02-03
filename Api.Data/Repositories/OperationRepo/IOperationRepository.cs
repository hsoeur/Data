using DataLib;

namespace Api.Data.Repositories.OperationRepo
{
    public interface IOperationRepository
    {
        Task<List<Operation>> GetCollectionOperationByProductUid(string productUid);
        Task<Operation?> GetPreviousOperationByProductUid(string productUid, int currentOperationNumber);
        Task<List<Operation>> GetCollectionOperationBasic(string productUid);
    }
}
