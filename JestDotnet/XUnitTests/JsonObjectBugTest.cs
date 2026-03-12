using System.Text.Json.Nodes;
using JestDotnet;
using Xunit;

namespace XUnitTests;

public class JsonObjectBugTest
{
    [Fact]
    public void JsonObjectShouldMatchSnapshot()
    {
        var obj = new JsonObject
        {
            ["Name"] = "test",
            ["Value"] = 42,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonObjectKeysShouldBeSortedAlphabetically()
    {
        var obj = new JsonObject
        {
            ["Zebra"] = 1,
            ["Apple"] = 2,
            ["Mango"] = 3,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void NestedJsonObjectKeysShouldBeSortedAlphabetically()
    {
        var obj = new JsonObject
        {
            ["Z"] = new JsonObject
            {
                ["Beta"] = 1,
                ["Alpha"] = 2,
            },
            ["A"] = "first",
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonObjectWithArrayValues()
    {
        var obj = new JsonObject
        {
            ["Items"] = new JsonArray(1, 2, 3),
            ["Name"] = "test",
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonObjectWithNullValues()
    {
        var obj = new JsonObject
        {
            ["Value"] = null,
            ["Name"] = "test",
        };

        obj.ShouldMatchSnapshot();
    }
}
