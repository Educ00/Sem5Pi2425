using System.Threading.Tasks;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public interface IUserRepository: IRepository<User, UserId> {
        Task<User> GetByEmailAsync(Email email);
        Task<User> GetByActivationTokenAsync(string token);
        Task<User> GetByPasswordResetToken(string token);
    }
}