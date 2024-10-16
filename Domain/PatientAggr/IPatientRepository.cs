using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.PatientAggr;

public interface IPatientRepository : IRepository<Patient, MedicalRecordsNumber> {
    
}