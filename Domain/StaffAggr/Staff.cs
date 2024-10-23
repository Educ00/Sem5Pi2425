using System;
using System.Collections.Generic;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr;

public class Staff : Entity<UserId> {
    public User User { get; private set; }
    public UniqueIdentifier UniqueIdentifier { get; private set; }
    public List<AvailableSlots> AvailableSlots { get; private set; }
    public Specialization Specialization { get; set; }

    protected Staff() { }

    public Staff(User user, List<AvailableSlots> availableSlots, UniqueIdentifier uniqueIdentifier, Specialization specialization) {
        if (!user.Role.Equals(Role.doctor) && !user.Role.Equals(Role.nurse) && !user.Role.Equals(Role.technician)) {
            throw new BusinessRuleValidationException("Invalid staff role,");
        }

        this.Id = user.Id;
        User = user ?? throw new ArgumentNullException(nameof(user));
        AvailableSlots = availableSlots ?? throw new ArgumentNullException(nameof(availableSlots));
        UniqueIdentifier = uniqueIdentifier;
        Specialization = specialization;
    }

    public void UpdateUniqueIdentifier(UniqueIdentifier uniqueIdentifier) {
        UniqueIdentifier = uniqueIdentifier ?? throw new ArgumentNullException(nameof(uniqueIdentifier));
    }
    
    public void UpdateAvailableSlots(List<AvailableSlots> availableSlots) {
        AvailableSlots = availableSlots ?? throw new ArgumentNullException(nameof(availableSlots));
    }
    
    public void UpdateSpecialization(Specialization specialization) {
        Specialization = specialization;
    }
    

    
}