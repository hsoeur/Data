using DataLib;

namespace Api.Data.Repositories.SequenceRepo
{
    public interface ISequenceRepository
    {
        Task<Sequence?> GetLatestValidSequenceByProductAsync(string productUid, int operationNumber);
    }
}
