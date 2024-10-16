using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class OperationTypeId : EntityId{
    public OperationTypeId(string value) : base(value) { }
    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static OperationTypeId NewOperationTypeId() {
        return new OperationTypeId(Guid.NewGuid().ToString());
    }
}