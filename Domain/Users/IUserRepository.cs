using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.Users {
    public interface IUserRepository: IRepository<User, UserId> {
    }
}