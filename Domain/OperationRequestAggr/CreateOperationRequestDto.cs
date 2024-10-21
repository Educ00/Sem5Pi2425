using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;

namespace Sem5Pi2425.Domain.OperationRequestAggr
{

    public class CreateOperationRequestDto
    {
        public string Deadline { get; set; }
        
        public string Priority { get; set; }
        
        public string Doctor { get; set; }
        
        public RegisterPatientDto Patient { get; set; }
        public CreateOperationTypeDto OperationType { get; set; }

        public CreateOperationRequestDto(string deadline, string priority, string doctor,
            RegisterPatientDto patient, CreateOperationTypeDto operationType) {
            Deadline = deadline;
            Priority = priority;
            Doctor = doctor;
            Patient = patient;
            OperationType = operationType;
        }

    }
}