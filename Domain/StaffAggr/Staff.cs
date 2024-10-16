using System.Collections.Generic;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr;

public class Staff : Entity<UniqueIdentifier> {
    public User User { get; private set; }
    public List<AvailableSlots> AvailableSlots { get; private set; }
    public Specialization Specialization { get; private set; }

    protected Staff() { }

    public Staff(User user, List<AvailableSlots> availableSlots, UniqueIdentifier uniqueIdentifier, Specialization specialization) {
        if (!user.Role.Equals(Role.doctor) && !user.Role.Equals(Role.nurse) && !user.Role.Equals(Role.technician)) {
            throw new BusinessRuleValidationException("Invalid staff role,");
        }

        this.User = user;
        this.AvailableSlots = availableSlots;   
        this.Id = uniqueIdentifier;
        this.Specialization = specialization;
    }
}