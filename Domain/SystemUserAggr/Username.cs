using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class Username : IValueObject{
        public string Value {
            get;
            private set;
        }

        protected Username() {}
        public Username(string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("Username cannot be empty");
            }
            this.Value = value;
        }
    }
}