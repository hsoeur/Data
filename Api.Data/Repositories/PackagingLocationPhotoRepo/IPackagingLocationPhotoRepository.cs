using DataLib;

namespace Api.Data.Repositories.PackagingLocationPhotoRepo
{
    public interface IPackagingLocationPhotoRepository
    {
        Task<IEnumerable<PackagingLocationPhoto>> GetCollectionPackagingLocationPhotos();
        Task CreatePackagingLocationPhoto(PackagingLocationPhoto photo);
        Task<PackagingLocationPhoto?> GetPackagingLocationPhotoByArticleUidAndUseCase(string articleUid, string useCase);
    }
}
