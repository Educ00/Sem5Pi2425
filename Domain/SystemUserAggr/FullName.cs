using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class FullName : IValueObject {
        public string Value { get; private set; }

        protected FullName() { }

        public FullName(string value) {
            if (!IsValidFullname(value)) {
                throw new BusinessRuleValidationException("Invalid Full Name!");
            }
            this.Value = value;

        }

        public string FirstName() => Value.Split(' ')[0];
        public string LastName() => Value.Split(' ').Last();

        public override string ToString() => Value;

        public static bool IsValidFullname(string text) {
            return !string.IsNullOrEmpty(text) && text.Any(char.IsWhiteSpace);
        }
    }
}