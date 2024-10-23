using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.PatientAggr;

public class MedicalRecordsNumber : IValueObject {
    public string Value { get; private set; }

    protected MedicalRecordsNumber() { }

    public static MedicalRecordsNumber NewMedicalRecordsNumber() {
        var medicalRecordsNumber = new MedicalRecordsNumber {
            Value = DateTime.Now.ToString("yyyyMM") + "000001"
        };
        return medicalRecordsNumber;
    }

    public static MedicalRecordsNumber NewMedicalRecordsByString(string text) {
        var medicalRecordsNumber = new MedicalRecordsNumber {
            Value = text
        };
        return medicalRecordsNumber;
    }

    public DateTime GetDate() {
        var datePart = Value[..6];
        return DateTime.ParseExact(datePart, "yyyyMM", null);
    }

    public int GetNumber() {
        var numberPart = Value[6..];
        return int.Parse(numberPart);
    }

    public void IncrementSequencialNumber() {
        this.Value = GetDate().ToString("yyyyMM") + (GetNumber() + 1).ToString("D6");
    }
}