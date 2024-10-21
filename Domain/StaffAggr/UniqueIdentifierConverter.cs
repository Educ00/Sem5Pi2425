using Newtonsoft.Json;
using System;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class UniqueIdentifierConverter : JsonConverter<UniqueIdentifier>
    {
        public override UniqueIdentifier ReadJson(JsonReader reader, Type objectType, UniqueIdentifier existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Expect the value to be a string, then convert it to a UniqueIdentifier object
            var value = reader.Value?.ToString();
            return UniqueIdentifier.CreateFromString(value);
        }

        public override void WriteJson(JsonWriter writer, UniqueIdentifier value, JsonSerializer serializer)
        {
            // Convert the UniqueIdentifier object back to a string when serializing
            writer.WriteValue(value.Value);
        }
    }
}