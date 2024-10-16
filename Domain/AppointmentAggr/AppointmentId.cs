using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.AppointmentAggr;

public class AppointmentId : EntityId {
    public AppointmentId(string value) : base(value) { }
    protected override object createFromString(string text) {
        return text;
    }

    public override string AsString() {
        return (string)ObjValue;
    }

    public AppointmentId NewAppointmentId() {
        return new AppointmentId(Guid.NewGuid().ToString());
    }
}