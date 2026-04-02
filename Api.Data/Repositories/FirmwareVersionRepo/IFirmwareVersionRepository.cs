using DataLib;

namespace Api.Data.Repositories.FirmwareVersionRepo
{
    public interface IFirmwareVersionRepository
    {
        Task<FirmwareVersion?> GetFirmwareVersionByArticleUid(string articleUid);
    }
}
