using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<User>> GetCollectionUsers()
        {
            Log.Information("GetAllOperateurs called.");

            try
            {
                var operateurs = await _context.Users.ToListAsync();
                Log.Information("Retrieved {Count} operateurs.", operateurs.Count);

                return operateurs;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve operateurs. Exception: {Exception}", ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<ICollection<User>> GetCollectionCurrentUsers()
        {
            Log.Information("GetCurrentOperateurs called.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Sentinel exit dates representing active users
                var validExitDates = new[]
                {
                    new DateTimeOffset(9999,12,31,0,0,0, TimeSpan.Zero),
                    new DateTimeOffset(9999,1,1,0,0,0, TimeSpan.Zero)
                };

                var operateurs = await _context.Users
                    .AsNoTracking()
                    .Where(o => validExitDates.Contains(o.ExitDate) && o.Department == "PRO")
                    .ToListAsync();

                stopwatch.Stop();
                Log.Information("Retrieved {Count} current operateurs in {ElapsedMilliseconds} ms.", operateurs.Count, stopwatch.ElapsedMilliseconds);

                return operateurs;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Log.Error("Failed to retrieve current operateurs. Exception: {Exception}. Query executed in {ElapsedMilliseconds} ms.", ex, stopwatch.ElapsedMilliseconds);
                throw; // Rethrow the exception to be handled upstream
            }
        }

        public async Task<User> GetOperatorByOperatorUid(string uidOperateur)
        {
            Log.Information("GetOperateur called with uidOperateur: {UidOperateur}", uidOperateur);

            try
            {
                var operateur = await _context.Users
                    .Where(o => o.UserUid == uidOperateur)
                    .FirstOrDefaultAsync();

                if (operateur == null)
                {
                    Log.Warning("No operateur found with uidOperateur: {UidOperateur}", uidOperateur);
                }
                else
                {
                    Log.Information("Retrieved operateur with uidOperateur: {UidOperateur}", uidOperateur);
                }

                return operateur;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve operateur with uidOperateur: {UidOperateur}. Exception: {Exception}", uidOperateur, ex);
                throw; // Rethrow the exception to be handled upstream
            }
        }
    }
}