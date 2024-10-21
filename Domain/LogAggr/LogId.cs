using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.LogAggr;

public class LogId : EntityId{
    public LogId(string value) : base(value) { }

    protected override string createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public static LogId NewLogId() {
        return new LogId(Guid.NewGuid().ToString());
    }
}