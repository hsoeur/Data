using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.PackagingLocationPhotoRepo
{
    public class PackagingLocationPhotoRepository : IPackagingLocationPhotoRepository
    {
        private readonly AppDbContext _context;

        public PackagingLocationPhotoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PackagingLocationPhoto>> GetCollectionPackagingLocationPhotos()
        {
            try
            {
                return await _context.PackagingLocationPhotos.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve PackagingLocationPhotos.");
                throw;
            }
        }

        public async Task CreatePackagingLocationPhoto(PackagingLocationPhoto photo)
        {
            try
            {
                _context.PackagingLocationPhotos.Add(photo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create PackagingLocationPhoto: {@Photo}", photo);
                throw;
            }
        }

        public async Task<PackagingLocationPhoto?> GetPackagingLocationPhotoByArticleUidAndUseCase(string articleUid, string useCase)
        {
            Log.Information("Fetching PackagingLocationPhoto with ArticleUid: {ArticleUid} and UseCase: {UseCase}", articleUid, useCase);

            try
            {
                // ArticleUid sans suffixe éventuel
                var articleUidBase = articleUid.Split('-')[0];

                var packagingLocationPhoto = await _context.PackagingLocationPhotos
                    .Where(pa => pa.UseCase == useCase &&
                                (pa.ArticleUid == articleUid || pa.ArticleUid.StartsWith(articleUidBase)))
                    .FirstOrDefaultAsync();

                if (packagingLocationPhoto == null)
                {
                    Log.Warning("No PhotoArticle found for ArticleUid: {ArticleUid} (base: {ArticleUidBase}) and UseCase: {UseCase}", articleUid, articleUidBase, useCase);
                }
                else
                {
                    Log.Information("PhotoArticle found: {PhotoArticle}", packagingLocationPhoto);
                }

                return packagingLocationPhoto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching PhotoArticle for ArticleUid: {ArticleUid} and UseCase: {UseCase}", articleUid, useCase);
                throw;
            }
        }
    }
}
