using System.Collections.Generic;
using System.Threading.Tasks;
using DataLib;

namespace Api.Data.Repositories.MachineRepo
{
 public interface IMachineRepository
 {
 Task<ICollection<Machine>> GetAllMachines();
 Task<Machine> GetMachineByMachineUid(string machineUid);
 }
}
