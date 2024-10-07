using System;
using Newtonsoft.Json;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.Users {
    public class UserId : EntityId {
        
        [JsonConstructor]
        public UserId(Guid value) : base(value){
        }

        public UserId(string value) : base(value){
        }

        protected override object createFromString(string text)
        {
            return new Guid(text);
        }

        public override string AsString() {
            var obj = (Guid) base.ObjValue;
            return obj.ToString();
        }

        public Guid AsGuid() {
            return (Guid) base.ObjValue;
        }
    }
}