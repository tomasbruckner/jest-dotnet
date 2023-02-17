using System.Runtime.CompilerServices;
using System.Text.Json.JsonDiffPatch;
using JestDotnet.Core;
using JestDotnet.Core.Exceptions;
using JestDotnet.Core.Settings;

namespace JestDotnet
{
    public static class JestDotnetExtensions
    {
        public static void ShouldMatchSnapshot(
            this object actual,
            string hint = "",
            JsonDiffOptions diffOptions = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = ""
        )
        {
            var path = SnapshotSettings.CreatePath((sourceFilePath, memberName, hint));
            var snapshot = SnapshotResolver.GetSnapshotData(path);

            if (string.IsNullOrEmpty(snapshot))
            {
                SnapshotUpdater.TryUpdateMissingSnapshot(path, actual);
                return;
            }

            var (isValid, message) = SnapshotComparer.CompareSnapshots(snapshot, actual, diffOptions);
            if (!isValid)
            {
                SnapshotUpdater.TryUpdateSnapshot(path, actual, message);
            }
        }

        public static void ShouldMatchInlineSnapshot(
            this object actual,
            string inlineSnapshot,
            JsonDiffOptions diffOptions = null
        )
        {
            var (isValid, message) = SnapshotComparer.CompareSnapshots(inlineSnapshot, actual, diffOptions);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }

        public static void ShouldMatchObject(this object actual, object expected, JsonDiffOptions diffOptions = null)
        {
            var (isValid, message) = SnapshotComparer.CompareSnapshots(expected, actual, diffOptions);
            if (!isValid)
            {
                throw new SnapshotMismatch(message);
            }
        }
    }
}
