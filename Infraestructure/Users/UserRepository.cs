using Sem5Pi2425.Domain.Users;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.Users {
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository  {

        public UserRepository(Sem5Pi2425DbContext context) : base(context.Users) { }
    }
}