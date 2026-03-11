using System.Collections.Generic;
using JestDotnet;
using Xunit;

namespace XUnitTests;

public class CircularReferenceTests
{
    // --- Circular references serialize with null (IgnoreCycles) ---

    [Fact]
    public void DirectSelfReference()
    {
        var node = new TreeNode { Name = "root" };
        node.Parent = node;

        JestAssert.ShouldMatchSnapshot(node);
    }

    [Fact]
    public void MutualReference()
    {
        var a = new TreeNode { Name = "A" };
        var b = new TreeNode { Name = "B" };
        a.Parent = b;
        b.Parent = a;

        JestAssert.ShouldMatchSnapshot(a);
    }

    [Fact]
    public void LongerCycle()
    {
        var a = new TreeNode { Name = "A" };
        var b = new TreeNode { Name = "B" };
        var c = new TreeNode { Name = "C" };
        a.Parent = b;
        b.Parent = c;
        c.Parent = a;

        JestAssert.ShouldMatchSnapshot(a);
    }

    [Fact]
    public void CycleThroughList()
    {
        var node = new GraphNode { Name = "root" };
        node.Children.Add(node);

        JestAssert.ShouldMatchSnapshot(node);
    }

    [Fact]
    public void CycleThroughDictionary()
    {
        var node = new DictNode { Name = "root" };
        node.Links["self"] = node;

        JestAssert.ShouldMatchSnapshot(node);
    }

    // --- Deep but non-circular should still work ---

    [Fact]
    public void DeepNonCircularChain()
    {
        var current = new TreeNode { Name = "leaf" };
        for (var i = 0; i < 20; i++)
        {
            current = new TreeNode { Name = $"node{i}", Parent = current };
        }

        JestAssert.ShouldMatchSnapshot(current);
    }

    // --- Shared reference (diamond, not cycle) should work ---

    [Fact]
    public void SharedReferenceNoCycle()
    {
        var shared = new TreeNode { Name = "shared" };
        var parent = new TwoChildren
        {
            Left = shared,
            Right = shared,
        };

        JestAssert.ShouldMatchSnapshot(parent);
    }

    // --- Helper classes ---

    private class TreeNode
    {
        public string? Name { get; set; }
        public TreeNode? Parent { get; set; }
    }

    private class GraphNode
    {
        public string? Name { get; set; }
        public List<GraphNode> Children { get; set; } = [];
    }

    private class DictNode
    {
        public string? Name { get; set; }
        public Dictionary<string, DictNode> Links { get; set; } = new();
    }

    private class TwoChildren
    {
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }
    }
}
