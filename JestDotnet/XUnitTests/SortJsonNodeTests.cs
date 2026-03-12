using System.Linq;
using System.Text.Json.Nodes;
using JestDotnet.Core;
using Xunit;

namespace XUnitTests;

public class SortJsonNodeTests
{
    [Fact]
    public void SortJsonNodeSortsTopLevelKeys()
    {
        var node = JsonNode.Parse("""{"zebra":1,"apple":2,"mango":3}""");
        Serializer.SortJsonNode(node);

        var obj = node!.AsObject();
        var keys = obj.Select(p => p.Key).ToList();
        Assert.Equal(new[] { "apple", "mango", "zebra" }, keys);
    }

    [Fact]
    public void SortJsonNodeSortsNestedKeys()
    {
        var node = JsonNode.Parse("""{"z":{"beta":1,"alpha":2},"a":"first"}""");
        Serializer.SortJsonNode(node);

        var obj = node!.AsObject();
        var topKeys = obj.Select(p => p.Key).ToList();
        Assert.Equal(new[] { "a", "z" }, topKeys);

        var nested = obj["z"]!.AsObject();
        var nestedKeys = nested.Select(p => p.Key).ToList();
        Assert.Equal(new[] { "alpha", "beta" }, nestedKeys);
    }

    [Fact]
    public void SortJsonNodeSortsObjectsInsideArrays()
    {
        var node = JsonNode.Parse("""[{"z":1,"a":2},{"b":3,"a":4}]""");
        Serializer.SortJsonNode(node);

        var first = node![0]!.AsObject();
        Assert.Equal(new[] { "a", "z" }, first.Select(p => p.Key).ToList());

        var second = node[1]!.AsObject();
        Assert.Equal(new[] { "a", "b" }, second.Select(p => p.Key).ToList());
    }

    [Fact]
    public void SortJsonNodeHandlesNull()
    {
        // Should not throw
        Serializer.SortJsonNode(null);
    }

    [Fact]
    public void SortJsonNodeHandlesValueNode()
    {
        var node = JsonNode.Parse("42");
        // Should not throw
        Serializer.SortJsonNode(node);
    }
}
