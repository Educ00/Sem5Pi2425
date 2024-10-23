using System;
using System.Collections.Generic;

namespace Sem5Pi2425.Domain.OperationTypeAggr {

    public class OperationTypeDto
    {
        public OperationTypeId Id { get; set; }
        public DateTime Duration { get; set; }
        public Name Name { get; set; }
        public Description Description { get; set; }
        public List<string> NeededSpecializations { get; set; }
        public Boolean Active { get; set; }

        public OperationTypeDto(OperationType operationType) : this(operationType.Id, operationType.Duration, operationType.Name, operationType.Description, operationType.NeededSpecializations, operationType.Active) { }
        
        public OperationTypeDto(OperationTypeId id, DateTime duration, Name name, Description description,
            List<string> neededSpecializations, Boolean active) {
            Id = id;
            Duration = duration;
            Name = name;
            Description = description;
            NeededSpecializations = neededSpecializations;
            Active = active;
        }


    }
}