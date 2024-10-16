using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr;

public class PhoneNumber : IValueObject {
    public string Value { get; private set; }

    protected PhoneNumber() {}

    public PhoneNumber(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            throw new ArgumentException("Phone Number Cannot Be Empty");
        }
        if (!value.All(char.IsDigit)) {
            throw new ArgumentException("Phone Number Must Only Contain Numbers");
        }
        this.Value = value;
    }

    public override string ToString() => Value;
}