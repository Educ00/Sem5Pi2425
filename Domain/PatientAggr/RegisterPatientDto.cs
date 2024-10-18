using System;

namespace Sem5Pi2425.Domain.PatientAggr;

public class RegisterPatientDto {
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string BirthDate { get; set; }
    public string Gender { get; set; }
    public string EmergencyContactFullName { get; set; }
    public string EmergencyContactEmail { get; set; }
    public string EmergencyContactPhoneNumber { get; set; }
    public string MedicalConditions { get; set; }

}