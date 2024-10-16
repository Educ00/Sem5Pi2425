using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.PatientAggr;

public class EmergencyContact : IValueObject{
    public PhoneNumber PhoneNumber { get; private set; }
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    
    protected EmergencyContact() {}

    public EmergencyContact(PhoneNumber phoneNumber, FullName fullName, Email email) {
        this.PhoneNumber = phoneNumber;
        this.FullName = fullName;
        this.Email = email;
    }
}