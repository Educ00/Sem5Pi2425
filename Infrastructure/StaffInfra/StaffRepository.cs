using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.StaffInfra;

public class StaffRepository : BaseRepository<Staff, UserId>, IStaffRepository {
    public StaffRepository(Sem5Pi2425DbContext context) : base(context.Staffs) { }

    public new async Task<List<Staff>> GetAllAsync() {
        return await this.Objs
            .Include(p => p.User)
            .ToListAsync();
    }

    public new async Task<Staff> GetByIdAsync(UserId id) {
        return await this.Objs
            .Include(p => p.User)
            .FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public new async Task<List<Staff>> GetByIdsAsync(List<UserId> ids) {
        return await this.Objs
            .Include(p => p.User)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<Staff> GetByUsernameAsync(string username) {
        return await Objs
            .Include(p => p.User)
            .FirstOrDefaultAsync(x => x.User.Username.Value.Equals(username));
    }
}