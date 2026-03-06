using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XUnitTests.Helpers;

public sealed class SortedJObjectConverter : JsonConverter<JObject>
{
    public override JObject ReadJson(JsonReader reader, Type objectType, JObject? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        return JObject.Load(reader);
    }

    public override void WriteJson(JsonWriter writer, JObject? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        foreach (var property in value.Properties().OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            writer.WritePropertyName(property.Name);
            serializer.Serialize(writer, property.Value);
        }
        writer.WriteEndObject();
    }
}
