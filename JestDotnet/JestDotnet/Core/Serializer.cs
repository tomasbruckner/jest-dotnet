using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using JestDotnet.Core.Settings;

namespace JestDotnet.Core;

internal static class Serializer
{
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
            SortJsonNode(node);
            JsonSerializer.Serialize(writer, node, options);
        }
        else if (obj is JsonNode jsonNode)
        {
            SortJsonNode(jsonNode);
            JsonSerializer.Serialize(writer, jsonNode, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, obj, options);
        }

        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    internal static void SortJsonNode(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
            {
                var sorted = obj
                    .OrderBy(p => p.Key, StringComparer.Ordinal)
                    .Select(p => new KeyValuePair<string, JsonNode?>(p.Key, p.Value))
                    .ToList();

                // Detach all values from the object first to avoid parent conflicts
                foreach (var kvp in sorted)
                {
                    obj.Remove(kvp.Key);
                }

                // Re-add in sorted order
                foreach (var kvp in sorted)
                {
                    obj[kvp.Key] = kvp.Value;
                }

                // Recursively sort nested objects
                foreach (var kvp in obj)
                {
                    SortJsonNode(kvp.Value);
                }

                break;
            }
            case JsonArray arr:
            {
                foreach (var item in arr)
                {
                    SortJsonNode(item);
                }

                break;
            }
        }
    }
}
