
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.SystemUser;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.Users {
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository {

        public UserRepository(Sem5Pi2425DbContext context) : base(context.Users) {
        }

        public Task<User> GetByEmailAsync(Domain.SystemUser.Email email) { ;
            return Objs.FirstOrDefaultAsync(x => x.Email.Value.Equals(email.Value));
        }

        public Task<User> GetByActivationTokenAsync(string token) {
            return Objs.FirstOrDefaultAsync(x => x.ActivationToken.Equals(token));
        }

        public Task<User> GetByPasswordResetToken(string token) {
            return Objs.FirstOrDefaultAsync(x => x.PasswordRequestToken.Equals(token));
        }
    }
}