using System;
using System.IO;
using JestDotnet.Core;
using JestDotnet.Core.Exceptions;

namespace JestDotnet
{
    public static class JestDotnetExtensions
    {
        private const string SnapshotExtension = "snap";
        private const string SnapshotDirectory = "__snapshots__";
        private static readonly string SnapshotDotExtension = $".{SnapshotExtension}";

        public static void ShouldMatchSnapshot(
            this object actual,
            string hint = "",
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = ""
        )
        {
            var path = CreatePath(sourceFilePath, memberName, hint);
            var snapshot = SnapshotResolver.GetSnapshotData(path);

            if (string.IsNullOrEmpty(snapshot))
            {
                SnapshotUpdater.TryUpdateMissingSnapshot(path, actual);
                return;
            }

            var (isValid, message) = SnapshotComparer.CompareSnapshots(snapshot, actual);
            if (!isValid)
            {
                SnapshotUpdater.TryUpdateSnapshot(path, actual, message);
            }
        }

        public static void ShouldMatchInlineSnapshot(this object actual, string inlineSnapshot)
        {
            var (isValid, message) = SnapshotComparer.CompareSnapshots(inlineSnapshot, actual);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }

        public static void ShouldMatchObject(this object actual, object expected)
        {
            var (isValid, message) = SnapshotComparer.CompareSnapshots(expected, actual);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }

        private static string CreatePath(string sourceFilePath, string memberName, string hint)
        {
            var directoryName = $"{Path.GetDirectoryName(sourceFilePath)}/{SnapshotDirectory}";
            var fileName =
                $"{Path.GetFileNameWithoutExtension(sourceFilePath)}{memberName}{hint}{SnapshotDotExtension}";
            
            return $"{directoryName}/{fileName}";
        }
    }
}
