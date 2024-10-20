using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr;

public class PhoneNumber : IValueObject {
    public string Value { get; private set; }

    protected PhoneNumber() {}

    public PhoneNumber(string value) {
        if (!IsValidPhoneNumber(value)) {
            throw new BusinessRuleValidationException("Invalid Phone Number!");
        }
        this.Value = value;
    }

    public override string ToString() => Value;

    public static bool IsValidPhoneNumber(string value) {
        return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
    }
}