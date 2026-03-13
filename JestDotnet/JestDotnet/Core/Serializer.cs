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
    internal static string Serialize(object? obj)
    {
        var options = SnapshotSettings.CreateSerializerOptions();
        var writerOptions = new JsonWriterOptions
        {
            Indented = options.WriteIndented,
            NewLine = SnapshotSettings.NewLine,
            Encoder = options.Encoder,
        };

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, writerOptions);
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
        var result = Encoding.UTF8.GetString(stream.ToArray());

        var sorted = JsonNode.Parse(result);
        SortJsonNode(sorted);

        using var sortedStream = new MemoryStream();
        using var sortedWriter = new Utf8JsonWriter(sortedStream, writerOptions);
        JsonSerializer.Serialize(sortedWriter, sorted, options);
        sortedWriter.Flush();
        return Encoding.UTF8.GetString(sortedStream.ToArray()).Replace("\\u0022", "\\\"");
    }

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
}
