using System.Collections.Generic;

namespace Sem5Pi2425.Domain.PatientAggr;

public class EditPatientDto
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string BirthDate { get; set; }
    public string Gender { get; set; }
    public string EmergencyContactFullName { get; set; }
    public string EmergencyContactEmail { get; set; }
    public string EmergencyContactPhoneNumber { get; set; }
    public string MedicalConditions { get; set; }

    public EditPatientDto(string email, string fullName, string phoneNumber, string birthDate, string gender, string emergencyContactFullName, string emergencyContactEmail, string emergencyContactPhoneNumber, string medicalConditions)
    {
        this.Email = email;
        this.FullName = fullName;
        this.PhoneNumber = phoneNumber;
        this.BirthDate = birthDate;
        this.Gender = gender;
        this.EmergencyContactFullName = emergencyContactFullName;
        this.EmergencyContactEmail = emergencyContactEmail;
        this.EmergencyContactPhoneNumber = emergencyContactPhoneNumber;
        this.MedicalConditions = medicalConditions;
    }

}