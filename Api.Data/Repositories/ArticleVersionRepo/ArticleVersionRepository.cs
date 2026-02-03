using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.ArticleVersionRepo
{
    public class ArticleVersionRepository : IArticleVersionRepository
    {
        private readonly AppDbContext _context;

        public ArticleVersionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ArticleVersion?> GetArticleVersionByArticleUid(string articleUid)
        {
            Log.Information("GetVersionByUidArticle called with UIDArticle: {UidArticle}", articleUid);

            try
            {
                var version = await _context.ArticleVersions
                    .FirstOrDefaultAsync(a => a.ArticleUid == articleUid);

                if (version == null)
                {
                    Log.Warning("No VersionArticle found for UIDArticle: {UidArticle}", articleUid);
                }
                else
                {
                    Log.Information("Retrieved VersionArticle for UIDArticle: {UidArticle}", articleUid);
                }

                return version;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve VersionArticle for UIDArticle: {UidArticle}", articleUid);
                throw;
            }
        }
    }
}
