using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.OperationRequestAggr;

public class OperationRequestId : EntityId{
    public OperationRequestId(string value) : base(value) { }
    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static OperationRequestId NewOperationRequestId() {
        return new OperationRequestId(Guid.NewGuid().ToString());
    }
}