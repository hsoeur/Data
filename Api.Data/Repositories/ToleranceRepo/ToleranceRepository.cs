using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.ToleranceRepo
{
    public class ToleranceRepository : IToleranceRepository
    {
        private readonly AppDbContext _context;

        public ToleranceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateTolerance(Tolerance tolerance)
        {
            Log.Information("CreateTolerance called with tolerance: {@Tolerance}", tolerance);

            try
            {
                _context.Tolerances.Add(tolerance);
                await _context.SaveChangesAsync();
                Log.Information("Successfully created tolerance: {@Tolerance}", tolerance);
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException, "Failed to create tolerance: {@Tolerance}. Exception: {Exception}", tolerance, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<List<Tolerance>> GetCollectionTolerances()
        {
            Log.Information("GetCollectionAllTolerances called :");

            try
            {
                var tolerances = await _context.Tolerances
                    .OrderBy(u => u.ProductUid)
                    .ThenBy(u => u.OperationNumber)
                    .ThenBy(u => u.Label)
                    .ToListAsync();

                Log.Information("Retrieved {Count} tolerances", tolerances.Count);

                return tolerances;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve tolerances. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<List<Tolerance>> GetCollectionTolerancesByProductUidByOperationByMachineLatestOne(string productUid, int operationNumber, string machine)
        {
            try
            {
                // Log d'information
                Log.Information("Fetching tolerances for ProductUid: {ProductUid}, OperationNumber: {OperationNumber}, Machine: {Machine}",
                                productUid, operationNumber, machine);

                // Enregistrements exacts
                var exactMatches = await _context.Tolerances
                    .Where(t => t.ProductUid == productUid &&
                                t.OperationNumber == operationNumber &&
                                t.MachineUid == machine)
                    .ToListAsync();

                // Enregistrements MachineUid commençant par "MO03"
                var mo03Matches = await _context.Tolerances
                    .Where(t => t.ProductUid == productUid &&
                                t.OperationNumber == operationNumber &&
                                EF.Functions.Like(t.MachineUid, "MO03%"))
                    .ToListAsync();

                // Fusion
                var mergedResults = exactMatches
                    .Union(mo03Matches)
                    .ToList();

                // Filtrer EndDate à la valeur max
                var filteredResults = mergedResults
                    .Where(t => t.EndDate == new DateTimeOffset(9999,12,31,23,59,59, TimeSpan.Zero))
                    .ToList();

                // Distinct par Label, priorisant MachineUid exact
                var distinctByLabel = filteredResults
                    .GroupBy(t => t.Label)
                    .Select(group =>
                        group.FirstOrDefault(t => t.MachineUid == machine) ?? group.First()
                    )
                    .ToList();

                // Log du succès
                Log.Information("Fetched {Count} tolerances (Exact Matches: {ExactCount}, MO03 Matches: {MO03Count}).",
                                distinctByLabel.Count, exactMatches.Count, mo03Matches.Count);

                return distinctByLabel;
            }
            catch (Exception ex)
            {
                // Log de l'erreur
                Log.Error(ex, "An error occurred while fetching tolerances for ProductUid: {ProductUid}, OperationNumber: {OperationNumber}, Machine: {Machine}",
                          productUid, operationNumber, machine);
                throw;
            }
        }
    }
}
