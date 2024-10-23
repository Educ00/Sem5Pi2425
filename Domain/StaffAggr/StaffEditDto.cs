using System.Collections.Generic;
#nullable enable

namespace Sem5Pi2425.Domain.StaffAggr;

public class StaffEditDto
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? uniqueIdentifier { get; set; }

    public List<string>? availableSlotsList { get; set; }
    public string? specialization { get; set; }
    
    public StaffEditDto(string email, string fullName, string phoneNumber, string uniqueIdentifier, List<string> availableSlotsList, string specialization)
    {
        this.Email = email;
        this.FullName = fullName;
        this.PhoneNumber = phoneNumber;
        this.uniqueIdentifier = uniqueIdentifier;
        this.availableSlotsList = availableSlotsList;
        this.specialization = specialization;
    }
}