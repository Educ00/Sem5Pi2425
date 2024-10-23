using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.StaffAggr;

public class UniqueIdentifier : IValueObject {
    public string Value { get; private set; }
    
    protected UniqueIdentifier() { }
    
    public UniqueIdentifier(char rolePrefix, int sequentialNumber) {
        this.Value = rolePrefix + DateTime.Now.ToString("yyyyMM") + sequentialNumber.ToString("D5");
    }

    public UniqueIdentifier(string dtoUniqueIdentifier)
    {
        this.Value = dtoUniqueIdentifier;
    }

    public static UniqueIdentifier CreateFromString(string text) {
        var temp = new UniqueIdentifier {
            Value = text
        };
        return temp;
    }
}