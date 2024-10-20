using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.PatientInfra;

public class PatientRepository : BaseRepository<Patient, UserId>, IPatientRepository {
    public PatientRepository(Sem5Pi2425DbContext context) : base(context.Patients) { }


    public new async Task<List<Patient>> GetAllAsync() {
        return await this.Objs
            .Include(p => p.User)
            .ToListAsync();
    }

    public new async Task<Patient> GetByIdAsync(UserId id) {
        return await this.Objs
            .Include(p => p.User)
            .FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public new async Task<List<Patient>> GetByIdsAsync(List<UserId> ids) {
        return await this.Objs
            .Include(p => p.User)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<Patient> GetByUsernameAsync(string username) {
        return await Objs
            .Include(p => p.User)
            .FirstOrDefaultAsync(x => x.User.Username.Value.Equals(username));
    }
}