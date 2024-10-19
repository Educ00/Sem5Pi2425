using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr;

public interface IStaffRepository : IRepository<Staff, UserId> {

    void Edit(Staff staff, Specialization specialization)
    {
        Staff staffToEdit = GetByIdAsync(staff.Id).Result;
        staffToEdit.Specialization = specialization;
        AddAsync(staffToEdit);
    }
   
}