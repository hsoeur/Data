using DataLib;

namespace Api.Data.Repositories.ArticleVersionRepo
{
    public interface IFirmwareVersionRepository
    {
        Task<FirmwareVersion?> GetFirmwareVersionByArticleUid(string articleUid);
    }
}
