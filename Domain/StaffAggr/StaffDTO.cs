using System.Collections.Generic;
using Newtonsoft.Json;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

public class StaffDTO
{
    public User User { get; set; }
    public string Id { get; set; }

    [JsonConverter(typeof(UniqueIdentifierConverter))]
    public UniqueIdentifier UniqueIdentifier { get; set; }

    public List<AvailableSlots> AvailableSlotsList { get; set; }
    public Specialization Specialization { get; set; }

    [JsonConstructor]  // This constructor will be used for deserialization
    public StaffDTO(User user, string id, UniqueIdentifier uid, List<AvailableSlots> availableSlots, Specialization specialization)
    {
        User = user;
        Id = id;
        UniqueIdentifier = uid;
        AvailableSlotsList = availableSlots;
        Specialization = specialization;
    }

    // Constructor that takes a Staff object
    public StaffDTO(Staff staff)
    {
        User = staff.User;
        Id = staff.Id.ToString();
        UniqueIdentifier = staff.UniqueIdentifier;
        AvailableSlotsList = staff.AvailableSlots;
        Specialization = staff.Specialization;
    }
}