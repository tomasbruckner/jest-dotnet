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
            var serialized = Serializer.Serialize(actualObject);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, serialized);
        }

        internal static string CreatePath((string sourceFilePath, string memberName, string hint) args)
        {
            var (sourceFilePath, memberName, hint) = args;
            var directoryName = $"{Path.GetDirectoryName(sourceFilePath)}/{SnapshotSettings.SnapshotDirectory}";
            var fileName =
                $"{Path.GetFileNameWithoutExtension(sourceFilePath)}{memberName}{hint}{SnapshotSettings.CreateSnapshotDotExtension()}";

            return $"{directoryName}/{fileName}";
        }
    }
}
