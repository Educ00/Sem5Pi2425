using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.PatientAggr;

public class MedicalRecordsNumber : EntityId{
    public MedicalRecordsNumber(object value) : base(value) { }
    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static MedicalRecordsNumber NewMedicalRecordsNumber(string text) {
        return new MedicalRecordsNumber(text);
    }
}