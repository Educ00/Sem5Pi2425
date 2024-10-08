using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUser {
    public class UserId : EntityId {
        private UserId(string value) : base(value) { }

        protected override object createFromString(string text) {
            return text;
        }

        public override string AsString() {
            return (string)ObjValue;
        }

        public static UserId NewUserId() {
            return new UserId(Guid.NewGuid().ToString());
        }
    }
}