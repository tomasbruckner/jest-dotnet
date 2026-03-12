using JestDotnet;
using JestDotnet.Core.Settings;
using Xunit;

namespace XUnitTests;

public class PreSerializerTests
{
    [Fact]
    public void ShouldSerializeCustomTypeViaPreSerializer()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(obj => obj.ToJson());
        try
        {
            var obj = new CustomSerializable("Alice", 30);
            JestAssert.ShouldMatchInlineSnapshot(obj, """
                {
                  "name": "Alice",
                  "age": 30
                }
                """);
        }
        finally
        {
            SnapshotSettings.ClearPreSerializers();
        }
    }
}

public class CustomSerializable
{
    public string Name { get; }
    public int Age { get; }

    public CustomSerializable(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string ToJson() => $$"""{"name": "{{Name}}", "age": {{Age}}}""";
}
