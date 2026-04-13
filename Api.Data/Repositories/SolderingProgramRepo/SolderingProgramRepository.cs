using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.SolderingProgramRepo
{

    public class SolderingProgramRepository : ISolderingProgramRepository
    {
        private readonly AppDbContext _context;

        public SolderingProgramRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SolderingProgram>> GetCollectionSolderingPrograms()
        {
            try
            {
                Log.Information("Attempting to retrieve soldering programs.");

                var programs = await _context.SolderingPrograms
                    .AsNoTracking()
                    .OrderBy(sp => sp.ProductUid)
                    .ThenBy(sp => sp.Side)
                    .ThenByDescending(sp => sp.EndDate)
                    .ToListAsync();

                if (programs == null || !programs.Any())
                {
                    Log.Warning("No soldering programs found.");
                }
                else
                {
                    Log.Information("Successfully retrieved {Count} soldering programs.", programs.Count);
                }

                return programs;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving soldering programs.");
                throw new Exception("An error occurred while retrieving soldering programs.", ex);
            }
        }


        public async Task<List<SolderingProgram>> GetCollectionSolderingProgramsByKeywordSearch(string keyword)
        {
            try
            {
                Log.Information("Searching for soldering programs with keyword: {Keyword}", keyword);

                // Prepare numeric filter for Program if keyword is an int
                bool hasNumericProgram = int.TryParse(keyword, out var programValue);

                var query = _context.SolderingPrograms
                    .AsNoTracking()
                    .Where(ucp =>
                        ucp.ProductUid.Contains(keyword) ||
                        ucp.Side.Contains(keyword) ||
                        (hasNumericProgram && ucp.Program == programValue)
                    )
                    .OrderBy(u => u.ProductUid)
                    .ThenBy(u => u.Side)
                    .ThenByDescending(u => u.EndDate);

                var programs = await query.ToListAsync();

                if (!programs.Any())
                {
                    Log.Warning("No soldering programs found for the keyword: {Keyword}", keyword);
                }
                else
                {
                    Log.Information("Found {Count} soldering programs matching the keyword: {Keyword}", programs.Count, keyword);
                }

                return programs;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while searching for soldering programs with keyword: {Keyword}", keyword);
                throw new Exception("An error occurred while searching for soldering programs.", ex);
            }
        }
        public async Task<List<SolderingProgram>> GetCollectionSolderingProgramsActiveOnes()
        {
            try
            {
                Log.Information("Attempting to retrieve active soldering programs.");

                var now = DateTimeOffset.Now;

                var solderingProgramsActiveOnes = await _context.SolderingPrograms
                    .AsNoTracking()
                    .Where(u => u.EndDate > now)
                    .OrderBy(u => u.ProductUid)
                    .ThenBy(u => u.Side)
                    .ToListAsync();

                Log.Information(
                    "Successfully retrieved {Count} active soldering programs.",
                    solderingProgramsActiveOnes.Count
                );

                return solderingProgramsActiveOnes;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving active soldering programs.");
                throw new Exception("An error occurred while retrieving active soldering programs.", ex);
            }
        }

        public async Task<List<SolderingProgram>> GetCollectionSolderingProgramsByKeywordActiveOnes(string keyword)
        {
            try
            {
                var now = DateTimeOffset.Now;
                bool hasNumericProgram = int.TryParse(keyword, out var programValue);

                var activePrograms = await _context.SolderingPrograms
                    .AsNoTracking()
                    .Where(ucp =>
                        ucp.EndDate > now &&
                        (
                            ucp.ProductUid.Contains(keyword) ||
                            ucp.Side.Contains(keyword) ||
                            (hasNumericProgram && ucp.Program == programValue)
                        )
                    )
                    .OrderBy(u => u.ProductUid)
                    .ThenBy(u => u.Side)
                    .ToListAsync();

                Log.Information("Retrieved {Count} soldering programs by keyword: {Keyword}", activePrograms.Count, keyword);

                return activePrograms;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while searching for active soldering programs with keyword: {Keyword}", keyword);
                throw new Exception("An error occurred while searching for active soldering programs.", ex);
            }
        }

        public async Task<SolderingProgram> UpdateSolderingProgram(SolderingProgram solderingProgram)
        {
            try
            {
                var oldUcp = await _context.SolderingPrograms.FirstOrDefaultAsync(ucp => ucp.SolderingProgramUid == solderingProgram.SolderingProgramUid);
                if (oldUcp == null)
                {
                    throw new InvalidOperationException("Soldering program not found");
                }

                bool hasProgramChanged = oldUcp.Program != solderingProgram.Program || oldUcp.Side != solderingProgram.Side;

                if (hasProgramChanged)
                {
                    oldUcp.EndDate = DateTimeOffset.Now;

                    _context.Entry(oldUcp).State = EntityState.Modified;

                    var newSolderingProgram = new SolderingProgram
                    {
                        SolderingProgramUid = Guid.NewGuid().ToString(),
                        ProductUid = solderingProgram.ProductUid,
                        Program = solderingProgram.Program,
                        StartDate = DateTimeOffset.Now.AddSeconds(1),
                        EndDate = DateTimeOffset.Parse("9999-12-31T23:59:59Z"),
                        UserUid = solderingProgram.UserUid,
                        Side = solderingProgram.Side,
                    };

                    _context.SolderingPrograms.Add(newSolderingProgram);
                    await _context.SaveChangesAsync();
                    return newSolderingProgram;
                }
                else
                {
                    // Update metadata only
                    oldUcp.UserUid = solderingProgram.UserUid;
                    _context.Entry(oldUcp).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return oldUcp;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the soldering program.", ex);
            }
        }

        public async Task<SolderingProgram> CreateSolderingProgram(SolderingProgram solderingProgram)
        {
            try
            {
                if (solderingProgram == null)
                {
                    throw new ArgumentNullException(nameof(solderingProgram), "Soldering program cannot be null.");
                }

                Log.Information("Creating a new soldering program with ProductUid: {ProductUid}", solderingProgram.ProductUid);

                _context.SolderingPrograms.Add(solderingProgram);
                await _context.SaveChangesAsync();

                Log.Information("Successfully created a new soldering program with UID: {SolderingProgramUid}", solderingProgram.SolderingProgramUid);
                return solderingProgram;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a new soldering program.");
                throw new InvalidOperationException("An error occurred while creating the soldering program.", ex);
            }
        }

        public async Task<List<SolderingProgram>> GetSolderingProgramByManufacturingOrderUid(int manufacturingOrderUid)
        {
            try
            {
                var of = await _context.ManufacturingOrders.FirstOrDefaultAsync(o => o.ManufacturingOrderUid == manufacturingOrderUid);
                if (of == null)
                {
                    return new List<SolderingProgram>();
                }

                var now = DateTimeOffset.Now;
                var solderingPrograms = await _context.SolderingPrograms
                    .Where(soldering => soldering.ProductUid == of.ProductUid && soldering.EndDate > now)
                    .OrderByDescending(s => s.StartDate)
                    .ToListAsync();

                return solderingPrograms;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }

}
