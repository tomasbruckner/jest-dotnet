using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using JestDotnet.Core.Settings;

namespace JestDotnet.Core;

internal static class Serializer
{
    internal static void SortJsonNode(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                var sorted = obj.OrderBy(p => p.Key, StringComparer.Ordinal).ToList();
                obj.Clear();
                foreach (var (key, value) in sorted)
                {
                    obj.Add(key, value);
                    SortJsonNode(value);
                }
                break;
            case JsonArray arr:
                foreach (var element in arr)
                {
                    SortJsonNode(element);
                }
                break;
        }
    }

    internal static string Serialize(object? obj)
    {
        var options = SnapshotSettings.CreateSerializerOptions();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = options.WriteIndented,
            NewLine = SnapshotSettings.NewLine,
            Encoder = options.Encoder,
        });
        if (obj is not null && SnapshotSettings.TryGetPreSerializer(obj.GetType(), out var preSerializer))
        {
            var json = preSerializer!(obj);
            var node = JsonNode.Parse(json);
            JsonSerializer.Serialize(writer, node, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, obj, options);
        }

        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
