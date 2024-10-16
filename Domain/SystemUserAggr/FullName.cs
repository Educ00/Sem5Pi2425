using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    
    public class FullName : IValueObject {
        public string Value { get; private set; }

        protected FullName() { }

        public FullName(string value) {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException("Full name cannot be empty");
            }

            if (!value.Any(char.IsWhiteSpace)) {
                throw new ArgumentException("Full Name must have first and last name!");
            }

            this.Value = value;
        }

        public string FirstName() => Value.Split(' ')[0];
        public string LastName() => Value.Split(' ').Last();

        public override string ToString() => Value;
    }
}