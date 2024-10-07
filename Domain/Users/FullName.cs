using System;
using System.Linq;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.Users {
    public class FullName : IValueObject {
        public string[] Value {
            get;
            private set;
        }
        
        protected FullName(){}

        public FullName(string value) {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException("Full name cannot be empty");
            }

            this.Value = value.Any(char.IsWhiteSpace)
                ? this.Value = value.Split(' ')
                : throw new ArgumentException("Full Name deve ter primeiro e último nome! ->" + value);
        }

        public string FirstName() {
            return this.Value[0];
        }

        public string LastName() {
            return this.Value[^1];
        }
    }
}