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

    [Fact]
    public void PreSerializerShouldPreserveKeyOrder()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(
            obj => $$"""{"zName": "{{obj.Name}}", "aAge": {{obj.Age}}}""");
        try
        {
            var obj = new CustomSerializable("Bob", 25);
            JestAssert.ShouldMatchInlineSnapshot(obj, """
                {
                  "zName": "Bob",
                  "aAge": 25
                }
                """);
        }
        finally
        {
            SnapshotSettings.ClearPreSerializers();
        }
    }

    [Fact]
    public void ObjectsWithoutPreSerializerUseDefaultSerialization()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(obj => obj.ToJson());
        try
        {
            var person = new { FirstName = "Alice", Age = 30 };
            JestAssert.ShouldMatchInlineSnapshot(person, """
                {
                  "Age": 30,
                  "FirstName": "Alice"
                }
                """);
        }
        finally
        {
            SnapshotSettings.ClearPreSerializers();
        }
    }

    [Fact]
    public void ClearPreSerializersShouldRemoveAllRegistrations()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(obj => obj.ToJson());
        SnapshotSettings.ClearPreSerializers();

        var obj = new CustomSerializable("Alice", 30);
        JestAssert.ShouldMatchInlineSnapshot(obj, """
            {
              "Age": 30,
              "Name": "Alice"
            }
            """);
    }

    [Fact]
    public void ShouldMatchObjectWorksWithPreSerializer()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(obj => obj.ToJson());
        try
        {
            var a = new CustomSerializable("Alice", 30);
            var b = new CustomSerializable("Alice", 30);
            JestAssert.ShouldMatchObject(a, b);
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
