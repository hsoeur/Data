using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.MachineRepo
{
    public class MachineRepository : IMachineRepository
    {
        private readonly AppDbContext _db;
        public MachineRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ICollection<Machine>> GetAllMachines()
        {
            Log.Information("GetAllMachines called.");
            try
            {
                var machines = await _db.Machines.AsNoTracking().ToListAsync();
                Log.Information("Retrieved {Count} machines.", machines.Count);
                return machines;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve machines. Exception: {Exception}", ex);
                throw;
            }
        }

        public async Task<Machine> GetMachineByMachineUid(string machineUid)
        {
            Log.Information("GetMachineByMachineUid called with machineUid: {MachineUid}", machineUid);
            try
            {
                var machine = await _db.Machines.AsNoTracking().FirstOrDefaultAsync(m => m.MachineUid == machineUid);
                if (machine == null)
                {
                    Log.Information("No machine found for machineUid: {MachineUid}", machineUid);
                }
                else
                {
                    Log.Information("Retrieved machine with machineUid: {MachineUid}", machineUid);
                }
                return machine;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to retrieve machine with machineUid: {MachineUid}. Exception: {Exception}", machineUid, ex);
                throw;
            }
        }
    }
}
