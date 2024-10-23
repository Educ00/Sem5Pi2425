using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.PatientAggr;

public class MedicalRecords : IValueObject{
    public MedicalRecordsNumber MedicalRecordsNumber { get; private set; }
    public string Value { get; private set; }
    
    protected MedicalRecords() { }

    public MedicalRecords(string value) {
        this.MedicalRecordsNumber = MedicalRecordsNumber.NewMedicalRecordsNumber();
        this.Value = value;
    }

    public void Write(string text) {
        this.Value = Value + "\n" + text;
        this.MedicalRecordsNumber.IncrementSequencialNumber();
    }
} 