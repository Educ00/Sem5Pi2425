using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.StaffInfra;

public class StaffRepository : BaseRepository<Staff, UserId>,IStaffRepository {
    public StaffRepository(Sem5Pi2425DbContext context) : base(context.Staffs) { }
}