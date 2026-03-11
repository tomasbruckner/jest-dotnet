using System.IO;
using System.Text;
using System.Text.Json;
using JestDotnet.Core.Settings;

namespace JestDotnet.Core;

internal static class Serializer
{
    internal static string Serialize(object obj)
    {
        var options = SnapshotSettings.CreateSerializerOptions();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = options.WriteIndented,
            NewLine = SnapshotSettings.NewLine,
            Encoder = options.Encoder,
        });
        JsonSerializer.Serialize(writer, obj, options);
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
