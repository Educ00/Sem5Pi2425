using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.StaffAggr;

public class UniqueIdentifier : EntityId{
    public UniqueIdentifier(string value) : base(value) { }
    
    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static UniqueIdentifier NewUniqueIdentifier() {
        return new UniqueIdentifier(Guid.NewGuid().ToString());
    }
}