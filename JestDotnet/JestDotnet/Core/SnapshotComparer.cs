using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using JestDotnet.Core.Settings;

namespace JestDotnet.Core
{
    internal static class SnapshotComparer
    {
        internal static (bool IsValid, string Message) CompareSnapshots<T>(
            T expectedObject,
            T actualObject,
            JsonDiffOptions diffOptions = null
        )
        {
            return Diff(
                Serializer.Serialize(expectedObject),
                Serializer.Serialize(actualObject),
                diffOptions
            );
        }

        internal static (bool IsValid, string Message) CompareSnapshots<T>(
            string serializedExpectedObject,
            T actualObject,
            JsonDiffOptions diffOptions = null
        )
        {
            return Diff(
                serializedExpectedObject,
                Serializer.Serialize(actualObject),
                diffOptions
            );
        }

        private static (bool IsValid, string Message) Diff(
            string expected,
            string actual,
            JsonDiffOptions diffOptions = null
        )
        {
            var expectedNode = JsonNode.Parse(expected);
            var actualNode = JsonNode.Parse(actual);

            var diff = expectedNode.Diff(
                actualNode,
                diffOptions ?? SnapshotSettings.DefaultCreateDiffOptions()
            );

            return (diff == null, diff?.ToJsonString());
        }
    }
}
