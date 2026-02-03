using DataLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Data.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetCollectionUsers();
        Task<ICollection<User>> GetCollectionCurrentUsers();
        Task<User> GetOperatorByOperatorUid(string operatorUid);
    }
}
