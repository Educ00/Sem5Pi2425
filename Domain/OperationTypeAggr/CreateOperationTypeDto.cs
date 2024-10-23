using System;

namespace Sem5Pi2425.Domain.OperationTypeAggr {

    public class CreateOperationTypeDto {
        public string Id { get; set; }
        public string Duration { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NeededSpecializations { get; set; }
        public string Active { get; set; }
        
        
        public CreateOperationTypeDto(string id, string duration, string name, string description,
            string neededSpecializations, string active) {
            Id = Guid.NewGuid().ToString() ;
            Duration = duration;
            Name = name;
            Description = description;
            NeededSpecializations = neededSpecializations;
            Active = active;
        }
    }
}