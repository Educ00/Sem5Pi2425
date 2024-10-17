using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.StaffInfra;

public class StaffRepository : BaseRepository<Staff, UserId>,IStaffRepository {
    private IStaffRepository _staffRepositoryImplementation;
    public StaffRepository(Sem5Pi2425DbContext context) : base(context.Staffs) { }

    public void Add(Staff staff)
    {
        _staffRepositoryImplementation.Add(staff);
    }

    public void Edit(Staff staff, Specialization specialization)
    {
        throw new System.NotImplementedException();
    }

    public void Edit(Staff staff)
    {
        throw new System.NotImplementedException();
    }
}