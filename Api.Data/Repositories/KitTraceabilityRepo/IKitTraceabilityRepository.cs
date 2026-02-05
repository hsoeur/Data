using DataLib;

namespace Api.Data.Repositories.KitTraceabilityRepo
{
    public interface IKitTraceabilityRepository
    {
        Task CreateKit(KitTraceability kit);
        Task<bool> AlreadyProcessed(int uidOf);
    }
}
