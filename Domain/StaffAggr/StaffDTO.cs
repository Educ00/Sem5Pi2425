using System.Collections.Generic;
using Newtonsoft.Json;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

public class StaffDTO
{
    public UserDto User { get; set; }
    

    [JsonConverter(typeof(UniqueIdentifierConverter))]
    public UniqueIdentifier UniqueIdentifier { get; set; }

    public List<AvailableSlots> AvailableSlotsList { get; set; }
    public Specialization Specialization { get; set; }

    [JsonConstructor]  // This constructor will be used for deserialization
    public StaffDTO(UserDto user,  UniqueIdentifier uid, List<AvailableSlots> availableSlots, Specialization specialization)
    {
        User = user;
        UniqueIdentifier = uid;
        AvailableSlotsList = availableSlots;
        Specialization = specialization;
    }


}