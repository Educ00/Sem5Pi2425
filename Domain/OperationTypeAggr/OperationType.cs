using System;
using System.Collections.Generic;
using System.Linq;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class OperationType : Entity<OperationTypeId> , IAggregateRoot{
    public DateTime Duration { get; private set; }
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public List<string> NeededSpecializations { get; private set; }

    protected OperationType() { }

    public OperationType(DateTime duration, Name name, Description description, List<Staff> neededSpecializations) {
        this.Id = OperationTypeId.NewOperationTypeId();
        this.Duration = duration;
        this.Name = name;
        this.Description = description;
        var temp = neededSpecializations.Select(a => a.Specialization.ToString()).ToList();
        this.NeededSpecializations = temp;
    }
}