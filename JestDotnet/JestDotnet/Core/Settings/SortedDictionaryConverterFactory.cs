using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JestDotnet.Core.Settings;

/// <summary>
///     Sorts dictionary keys alphabetically (as strings) during JSON serialization
/// </summary>
public class SortedDictionaryConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        var genericDef = typeToConvert.GetGenericTypeDefinition();

        return genericDef == typeof(Dictionary<,>) ||
               genericDef == typeof(IDictionary<,>) ||
               genericDef == typeof(IReadOnlyDictionary<,>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var args = typeToConvert.GetGenericArguments();
        var converterType = typeof(SortedDictionaryConverter<,>).MakeGenericType(args[0], args[1]);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class SortedDictionaryConverter<TKey, TValue> : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Remove our factory to avoid infinite recursion during deserialization
            var newOptions = new JsonSerializerOptions(options);
            var factory = newOptions.Converters.OfType<SortedDictionaryConverterFactory>().FirstOrDefault();
            if (factory is not null)
            {
                newOptions.Converters.Remove(factory);
            }

            return JsonSerializer.Deserialize(ref reader, typeToConvert, newOptions);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var entries = ((IEnumerable<KeyValuePair<TKey, TValue>>)value)
                .OrderBy(kvp => kvp.Key?.ToString(), StringComparer.Ordinal);

            writer.WriteStartObject();

            foreach (var kvp in entries)
            {
                var key = kvp.Key?.ToString() ?? string.Empty;
                writer.WritePropertyName(key);
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}
