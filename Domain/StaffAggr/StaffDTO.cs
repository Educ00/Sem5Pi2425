using System.Collections.Generic;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class StaffDTO
    {
        public UserId Id { get; set; }
        public UniqueIdentifier UniqueIdentifier { get; set; }
        public List<AvailableSlots> AvailableSlotsList { get; set; }
        public Specialization Specialization { get; set; }

        // Constructor that takes a Staff object
        public StaffDTO(Staff staff)
        {
            Id = staff.Id;
            UniqueIdentifier = staff.UniqueIdentifier;
            AvailableSlotsList = staff.AvailableSlots;
            Specialization = staff.Specialization;
        }

        // Constructor that takes individual properties
        public StaffDTO(UserId id, UniqueIdentifier uid, List<AvailableSlots> availableSlots, Specialization specialization)
        {
            Id = id;
            UniqueIdentifier = uid;
            AvailableSlotsList = availableSlots;
            Specialization = specialization;
        }
    }
}