namespace Sem5Pi2425.Domain.OperationRequestAggr
{

    public class CreateOperationRequestDto
    {
        public string Deadline { get; set; }
        
        public string Priority { get; set; }
        
        public string Doctor { get; set; }
        
        public string Patient { get; set; }
        public string OperationType { get; set; }

        public CreateOperationRequestDto(string deadline, string priority, string doctor,
            string patient, string operationType) {
            Deadline = deadline;
            Priority = priority;
            Doctor = doctor;
            Patient = patient;
            OperationType = operationType;
        }

    }
}