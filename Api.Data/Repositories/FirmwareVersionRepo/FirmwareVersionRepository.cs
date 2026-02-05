using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.ArticleVersionRepo
{
    public class FirmwareVersionRepository : IFirmwareVersionRepository
    {
        private readonly AppDbContext _context;

        public FirmwareVersionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FirmwareVersion?> GetFirmwareVersionByArticleUid(string articleUid)
        {
            Log.Information("GetFirmwareVersionByArticleUid called with UIDArticle: {UidArticle}", articleUid);

            try
            {
                var version = await _context.FirmwareVersions
                    .FirstOrDefaultAsync(a => a.ArticleUid == articleUid);

                if (version == null)
                {
                    Log.Warning("No FirmwareVersion found for UIDArticle: {UidArticle}", articleUid);
                }
                else
                {
                    Log.Information("Retrieved FirmwareVersion for UIDArticle: {UidArticle}", articleUid);
                }

                return version;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve FirmwareVersion for UIDArticle: {UidArticle}", articleUid);
                throw;
            }
        }
    }
}
