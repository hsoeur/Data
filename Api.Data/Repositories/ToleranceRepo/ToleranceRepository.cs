using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Api.Data.Dtos;

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

        public async Task<CopyToleranceBatchResponse> CopyBatchAsync(
            CopyToleranceBatchRequest request,
            CancellationToken cancellationToken)
        {
            var requestedUids = request.ToleranceUids
                .Where(uid => !string.IsNullOrWhiteSpace(uid))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (requestedUids.Length != request.ToleranceUids.Count)
                throw new InvalidToleranceSelectionException("La sélection contient un identifiant vide ou dupliqué.");

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(cancellationToken);

                var sourceProductExists = await _context.Articles
                    .AsNoTracking()
                    .AnyAsync(article => article.ArticleUid == request.SourceProductUid, cancellationToken);

                if (!sourceProductExists)
                    throw new SourceProductNotFoundException();

                var targetHasTolerances = await _context.Tolerances
                    .AsNoTracking()
                    .AnyAsync(tolerance => tolerance.ProductUid == request.TargetProductUid, cancellationToken);

                if (targetHasTolerances)
                    throw new TargetProductAlreadyExistsException();

                var sourceTolerances = await _context.Tolerances
                    .AsNoTracking()
                    .Where(tolerance => requestedUids.Contains(tolerance.ToleranceUid))
                    .ToListAsync(cancellationToken);

                var invalidUids = requestedUids
                    .Except(
                        sourceTolerances
                            .Where(tolerance => tolerance.ProductUid == request.SourceProductUid)
                            .Select(tolerance => tolerance.ToleranceUid),
                        StringComparer.Ordinal)
                    .ToArray();

                if (invalidUids.Length > 0)
                {
                    throw new InvalidToleranceSelectionException(
                        $"Tolérances inconnues ou n'appartenant pas au produit source : {string.Join(", ", invalidUids)}.");
                }

                var copies = sourceTolerances.Select(source => new Tolerance
                {
                    ToleranceUid = Guid.NewGuid().ToString(),
                    ProductUid = request.TargetProductUid,
                    OperationNumber = source.OperationNumber,
                    Label = source.Label,
                    MachineUid = source.MachineUid,
                    UnitUid = source.UnitUid,
                    Val1 = source.Val1,
                    Val2 = source.Val2,
                    Mode = source.Mode,
                    UserUid = request.UserUid,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }).ToList();

                _context.Tolerances.AddRange(copies);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new CopyToleranceBatchResponse(
                    request.SourceProductUid,
                    request.TargetProductUid,
                    copies.Count,
                    copies.Select(ToDto).ToArray());
            });
        }

        private static ToleranceDto ToDto(Tolerance tolerance) => new()
        {
            ToleranceUid = tolerance.ToleranceUid,
            ProductUid = tolerance.ProductUid,
            OperationNumber = tolerance.OperationNumber,
            Label = tolerance.Label,
            MachineUid = tolerance.MachineUid,
            UnitUid = tolerance.UnitUid,
            Val1 = tolerance.Val1,
            Val2 = tolerance.Val2,
            Mode = tolerance.Mode,
            UserUid = tolerance.UserUid,
            StartDate = tolerance.StartDate,
            EndDate = tolerance.EndDate
        };

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
                    .Where(t => t.EndDate == new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero))
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

        public async Task<Tolerance?> UpdateTolerance(string toleranceUid, Tolerance tolerance)
        {
            Log.Information("UpdateTolerance called with toleranceUid: {ToleranceUid}, tolerance: {@Tolerance}", toleranceUid, tolerance);

            try
            {
                var existingTolerance = await _context.Tolerances
                    .FirstOrDefaultAsync(t => t.ToleranceUid == toleranceUid);

                if (existingTolerance == null)
                {
                    Log.Warning("Tolerance with uid {ToleranceUid} not found", toleranceUid);
                    return null;
                }

                // Historisation: fermer l'ancienne tolérance en mettant EndDate à aujourd'hui
                existingTolerance.EndDate = DateTimeOffset.UtcNow;

                Log.Information("Closing existing tolerance {ToleranceUid} with EndDate: {EndDate}", toleranceUid, existingTolerance.EndDate);

                // Créer une nouvelle tolérance avec les nouvelles valeurs
                var newTolerance = new Tolerance
                {
                    ToleranceUid = Guid.NewGuid().ToString(),
                    ProductUid = tolerance.ProductUid,
                    OperationNumber = tolerance.OperationNumber,
                    Label = tolerance.Label,
                    MachineUid = tolerance.MachineUid,
                    UnitUid = tolerance.UnitUid,
                    Val1 = tolerance.Val1,
                    Val2 = tolerance.Val2,
                    Mode = tolerance.Mode,
                    UserUid = tolerance.UserUid,
                    StartDate = DateTimeOffset.UtcNow,
                    EndDate = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero)
                };

                _context.Tolerances.Add(newTolerance);
                await _context.SaveChangesAsync();

                Log.Information("Successfully created new tolerance: {@Tolerance}", newTolerance);

                return newTolerance;
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException, "Failed to update tolerance with uid {ToleranceUid}. Exception: {Exception}", toleranceUid, ex);
                throw;
            }
        }

        public async Task<List<Tolerance>> GetActiveTolerances()
        {
            Log.Information("GetActiveTolerances called");

            try
            {
                var currentDate = DateTimeOffset.UtcNow;

                var activeTolerances = await _context.Tolerances
                    .Where(t => t.StartDate <= currentDate && t.EndDate >= currentDate)
                    .OrderBy(u => u.ProductUid)
                    .ThenBy(u => u.OperationNumber)
                    .ThenBy(u => u.Label)
                    .ToListAsync();

                Log.Information("Retrieved {Count} active tolerances", activeTolerances.Count);

                return activeTolerances;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve active tolerances. Exception: {Exception}", ex);
                throw;
            }
        }
    }
}

