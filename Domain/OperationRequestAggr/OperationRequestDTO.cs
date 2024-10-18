using System;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationRequestAggr
{
    public class OperationRequestDTO
    {
        public OperationRequestId Id { get; set; }
 
        public DateTime Deadline { get; set; }
        
        public Priority Priority { get; set; }
        
        public User Doctor { get; set; }
        
        public Patient Patient { get; set; }
        
        public OperationRequestDTO(OperationRequest operationRequest) : this(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, operationRequest.Doctor, operationRequest.Patient) {}

        public OperationRequestDTO(OperationRequestId id, DateTime deadline, Priority priority, User doctor,
            Patient patient)
        {
            Id = id;
            Deadline = deadline;
            Priority = priority;
            Doctor = doctor;
            Patient = patient;
        }

    }

}