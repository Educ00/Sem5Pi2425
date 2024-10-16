using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.PatientAggr;

public class MedicalCondition : IValueObject{
    public string Value { get; private set; }
    
    protected MedicalCondition() {}

    public MedicalCondition(string condition) {
        this.Value = condition;
    }
}