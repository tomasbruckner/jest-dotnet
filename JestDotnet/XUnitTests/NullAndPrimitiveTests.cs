using System;
using JestDotnet;
using Xunit;

namespace XUnitTests;

public class NullAndPrimitiveTests
{
    // --- Null ---

    [Fact]
    public void NullPassedToShouldMatchInlineSnapshot()
    {
        JestAssert.ShouldMatchInlineSnapshot(null, "null");
    }

    [Fact]
    public void NullPassedToShouldMatchObject()
    {
        JestAssert.ShouldMatchObject(null, null);
    }

    // --- Primitives ---

    [Fact]
    public void IntSerialization()
    {
        JestAssert.ShouldMatchSnapshot(42);
    }

    [Fact]
    public void NegativeIntSerialization()
    {
        JestAssert.ShouldMatchSnapshot(-1);
    }

    [Fact]
    public void ZeroSerialization()
    {
        JestAssert.ShouldMatchSnapshot(0);
    }

    [Fact]
    public void LongSerialization()
    {
        JestAssert.ShouldMatchSnapshot(long.MaxValue);
    }

    [Fact]
    public void DoubleSerialization()
    {
        JestAssert.ShouldMatchSnapshot(3.14);
    }

    [Fact]
    public void BoolTrueSerialization()
    {
        JestAssert.ShouldMatchSnapshot(true);
    }

    [Fact]
    public void BoolFalseSerialization()
    {
        JestAssert.ShouldMatchSnapshot(false);
    }

    [Fact]
    public void StringSerialization()
    {
        JestAssert.ShouldMatchSnapshot("hello");
    }

    [Fact]
    public void EmptyStringSerialization()
    {
        JestAssert.ShouldMatchSnapshot("");
    }

    [Fact]
    public void StringWithSpecialCharacters()
    {
        JestAssert.ShouldMatchSnapshot("line1\nline2\ttab");
    }

    [Fact]
    public void StringWithQuotes()
    {
        JestAssert.ShouldMatchSnapshot("he said \"hi\"");
    }

    // --- Boxed primitives ---

    [Fact]
    public void BoxedInt()
    {
        object boxed = 42;
        JestAssert.ShouldMatchSnapshot(boxed);
    }

    [Fact]
    public void BoxedBool()
    {
        object boxed = true;
        JestAssert.ShouldMatchSnapshot(boxed);
    }

    [Fact]
    public void BoxedDouble()
    {
        object boxed = 2.718;
        JestAssert.ShouldMatchSnapshot(boxed);
    }

    // --- Other value types ---

    [Fact]
    public void CharSerialization()
    {
        JestAssert.ShouldMatchSnapshot('A');
    }

    [Fact]
    public void GuidSerialization()
    {
        JestAssert.ShouldMatchSnapshot(new Guid("12345678-1234-1234-1234-123456789abc"));
    }

    [Fact]
    public void DecimalSerialization()
    {
        JestAssert.ShouldMatchSnapshot(99.99m);
    }

    // --- Arrays of primitives ---

    [Fact]
    public void IntArraySerialization()
    {
        JestAssert.ShouldMatchSnapshot(new[] { 3, 1, 2 });
    }

    [Fact]
    public void StringArraySerialization()
    {
        JestAssert.ShouldMatchSnapshot(new[] { "b", "a", "c" });
    }

    [Fact]
    public void EmptyArraySerialization()
    {
        JestAssert.ShouldMatchSnapshot(Array.Empty<int>());
    }
}
