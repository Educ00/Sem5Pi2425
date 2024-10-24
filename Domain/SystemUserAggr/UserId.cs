using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class UserId : EntityId {
        public UserId(string value) : base(value) { }

        protected override string createFromString(string text) {
            return text;
        }

        public override string AsString() {
            return (string)ObjValue;
        }

        public static UserId NewUserId() {
            return new UserId(Guid.NewGuid().ToString());
        }
        
        public override string ToString()
        {
            return Value;
        }
    }
}