using Sem5Pi2425.Domain.SystemUser;
using Sem5Pi2425.Infrastructure;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infraestructure.Users {
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository  {

        public UserRepository(Sem5Pi2425DbContext context) : base(context.Users) { }
    }
}