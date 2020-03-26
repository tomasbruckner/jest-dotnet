using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JestDotnet.Core
{
    internal static class SnapshotComparer
    {
        internal static (bool IsValid, string Message) CompareSnapshots<T>(T expectedObject, T actualObject)
        {
            var serializedExpectedObject = JsonConvert.SerializeObject(expectedObject);
            var actualSerializedObject = JsonConvert.SerializeObject(actualObject);
            var isValid = serializedExpectedObject == actualSerializedObject;
            var message = isValid ? "" : "fail";

            return (isValid, message);
        }

        internal static (bool IsValid, string Message) CompareSnapshots<T>(
            string serializedExpectedObject,
            T actualObject
        )
        {
            var expectedToken = JToken.Parse(serializedExpectedObject);
            var actualToken = JToken.FromObject(actualObject);
            var isValid = JToken.DeepEquals(expectedToken, actualToken);
            var message = isValid ? "" : GetDiff(expectedToken, actualToken);

            return (isValid, message);
        }

        internal static string GetDiff(JToken expectedToken, JToken actualToken)
        {
            var diff = new JsonDiffPatch();
            var patch = diff.Diff(expectedToken, actualToken);

            return patch.ToString();
        }
    }
}
