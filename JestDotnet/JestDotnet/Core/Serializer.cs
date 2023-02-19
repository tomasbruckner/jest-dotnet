using JestDotnet.Core.Settings;

namespace JestDotnet.Core
{
    internal static class Serializer
    {
        internal static string Serialize(object obj)
        {
            var jsonSerializer = SnapshotSettings.CreateJsonSerializer();
            using var jsonWriter = SnapshotSettings.CreateJTokenWriter();
            jsonSerializer.Serialize(jsonWriter, obj);
            using var stringWriter = SnapshotSettings.CreateStringWriter();
            using var jsonTextWriter = SnapshotSettings.CreateTextWriter(stringWriter);
            jsonWriter.Token!.WriteTo(jsonTextWriter);
            
            return stringWriter.ToString();
        }
    }
}
