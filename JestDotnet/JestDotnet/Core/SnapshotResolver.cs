using System.IO;
using JestDotnet.Core.Settings;

namespace JestDotnet.Core
{
    internal static class SnapshotResolver
    {
        internal static string GetSnapshotData(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : "";
        }

        internal static void StoreSnapshotData(string path, object actualObject)
        {
            var jsonSerializer = SnapshotSettings.CreateJsonSerializer();
            using var jsonWriter = SnapshotSettings.CreateJTokenWriter();
            jsonSerializer.Serialize(jsonWriter, actualObject);
            using var stringWriter = SnapshotSettings.CreateStringWriter();
            using var jsonTextWriter = SnapshotSettings.CreateTextWriter(stringWriter);
            jsonWriter.Token!.WriteTo(jsonTextWriter);
            var serialized = stringWriter.ToString();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using var sw = SnapshotSettings.CreateStreamWriter(path);
            sw.Write(serialized);
        }

        internal static string CreatePath(
            (string sourceFilePath, string memberName, string hint) args
        )
        {
            var (sourceFilePath, memberName, hint) = args;
            var directoryName =
                $"{Path.GetDirectoryName(sourceFilePath)}/{SnapshotSettings.SnapshotDirectory}";
            var fileName =
                $"{Path.GetFileNameWithoutExtension(sourceFilePath)}{memberName}{hint}{SnapshotSettings.CreateSnapshotDotExtension()}";

            return $"{directoryName}/{fileName}";
        }
    }
}
