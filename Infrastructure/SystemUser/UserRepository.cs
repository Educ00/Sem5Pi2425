using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.SystemUser {
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository {
        public UserRepository(Sem5Pi2425DbContext context) : base(context.Users) { }

        public Task<User> GetByEmailAsync(string email) {
            return Objs.FirstOrDefaultAsync(x => x.Email.Value.Equals(email));
        }

        public Task<User> GetByActivationTokenAsync(string token) {
            return Objs.FirstOrDefaultAsync(x => x.ActivationToken.Equals(token));
        }

        public Task<User> GetByPasswordResetToken(string token) {
            return Objs.FirstOrDefaultAsync(x => x.PasswordRequestToken.Equals(token));
        }

        public Task<User> GetByUsername(string dtoUsername) {
            return Objs.FirstOrDefaultAsync(x => x.Username.Value.Equals(dtoUsername));
        }

        public Task<User> GetByPhoneNumber(string dtoPhoneNumber) {
            return Objs.FirstOrDefaultAsync(x => x.PhoneNumber.Value.Equals(dtoPhoneNumber));
        }

        public Task<User> GetByDeletionToken(string deletionToken) {
            return Objs.FirstOrDefaultAsync(x => x.DeletionToken.Equals(deletionToken));
        }

        public async Task<List<User>> GetWithRoleAsync(Role role) {
            return await Objs
                .Where(u => u.Role.Equals(role))
                .ToListAsync();
        }
    }
}