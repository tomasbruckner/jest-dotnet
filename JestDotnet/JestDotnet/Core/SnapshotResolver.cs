using System.IO;
using Newtonsoft.Json.Linq;

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
            var serialized = JToken.FromObject(actualObject).ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, serialized);
        }

        internal static string CreatePath(string sourceFilePath, string memberName, string hint)
        {
            var directoryName = $"{Path.GetDirectoryName(sourceFilePath)}/{SnapshotConstants.SnapshotDirectory}";
            var fileName =
                $"{Path.GetFileNameWithoutExtension(sourceFilePath)}{memberName}{hint}{SnapshotConstants.SnapshotDotExtension}";

            return $"{directoryName}/{fileName}";
        }
    }
}