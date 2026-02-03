using DataLib;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repositories.SequenceRepo
{
    public class SequenceRepository : ISequenceRepository
    {
        private readonly AppDbContext _context;

        public SequenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sequence?> GetLatestValidSequenceByProductAsync(string productUid, int operationNumber)
        {
            try
            {
                Log.Information("Recherche de la dernière séquence valide pour le produit : {ProductUid}", productUid);

                var latestSequence = await _context.Sequences
                    .Where(s => s.Product == productUid
                        && s.OperationNumber == operationNumber
                        && s.EndDate.Year == 9999)
                    .OrderByDescending(s => s.StartDate)
                    .FirstOrDefaultAsync();

                if (latestSequence == null)
                    Log.Warning("Aucune séquence valide trouvée pour le produit : {ProductUid}", productUid);

                return latestSequence;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la récupération de la séquence pour {ProductUid}", productUid);
                throw;
            }
        }
    }
}
