using System;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationRequestAggr;

public class OperationRequest : Entity<OperationRequestId>, IAggregateRoot {
    public DateTime Deadline { get; private set; }
    public Priority Priority { get; private set; }
    public User Doctor { get; private set; }
    public Patient Patient { get; private set; }
    public OperationType OperationType { get; private set; }

    protected OperationRequest() { }

    public OperationRequest(DateTime deadline, Priority priority, User doctor, Patient patient, OperationType operationType) {
        Id = OperationRequestId.NewOperationRequestId();
        Deadline = deadline;
        Priority = priority;
        Doctor = doctor;
        Patient = patient;
        OperationType = operationType;
    }
}