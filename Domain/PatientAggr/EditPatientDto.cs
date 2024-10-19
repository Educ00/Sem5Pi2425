using System.Collections.Generic;

namespace Sem5Pi2425.Domain.PatientAggr;

public class UpdatePatientDto
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public EmergencyContact EmergencyContact { get; set; }
    public List<string> MedicalConditions { get; set; }
    public List<string> Allergies { get; set; }
}