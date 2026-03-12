using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using JestDotnet;
using Xunit;

namespace XUnitTests;

public class JsonObjectEdgeCaseTests
{
    // --- Empty / minimal ---

    [Fact]
    public void EmptyJsonObject()
    {
        new JsonObject().ShouldMatchSnapshot();
    }

    [Fact]
    public void SinglePropertyJsonObject()
    {
        new JsonObject { ["Only"] = 1 }.ShouldMatchSnapshot();
    }

    // --- All JSON value types ---

    [Fact]
    public void AllJsonValueTypes()
    {
        var obj = new JsonObject
        {
            ["String"] = "hello",
            ["Number"] = 3.14,
            ["Integer"] = 42,
            ["BoolTrue"] = true,
            ["BoolFalse"] = false,
            ["Null"] = (JsonNode?)null,
        };

        obj.ShouldMatchSnapshot();
    }

    // --- Key sorting edge cases ---

    [Fact]
    public void NumericStringKeysSortLexicographically()
    {
        var obj = new JsonObject
        {
            ["10"] = "ten",
            ["2"] = "two",
            ["1"] = "one",
            ["20"] = "twenty",
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void CaseSensitiveKeySorting()
    {
        var obj = new JsonObject
        {
            ["abc"] = 1,
            ["ABC"] = 2,
            ["Abc"] = 3,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void PrefixKeySorting()
    {
        var obj = new JsonObject
        {
            ["NameLong"] = 1,
            ["Name"] = 2,
            ["NameA"] = 3,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void SpecialCharacterKeys()
    {
        var obj = new JsonObject
        {
            ["key with spaces"] = 1,
            ["key-with-dashes"] = 2,
            ["key.with.dots"] = 3,
            ["key_with_underscores"] = 4,
            [""] = 5,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void UnicodeKeys()
    {
        var obj = new JsonObject
        {
            ["ñ"] = 1,
            ["a"] = 2,
            ["ü"] = 3,
            ["z"] = 4,
        };

        obj.ShouldMatchSnapshot();
    }

    // --- Nesting ---

    [Fact]
    public void DeeplyNestedJsonObjects()
    {
        var obj = new JsonObject
        {
            ["Level1"] = new JsonObject
            {
                ["Level2"] = new JsonObject
                {
                    ["Level3"] = new JsonObject
                    {
                        ["B"] = 2,
                        ["A"] = 1,
                    },
                    ["A"] = "first",
                },
                ["A"] = "first",
            },
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonArrayContainingJsonObjects()
    {
        var obj = new JsonObject
        {
            ["Items"] = new JsonArray(
                new JsonObject
                {
                    ["Z"] = 1,
                    ["A"] = 2,
                },
                new JsonObject
                {
                    ["Beta"] = 3,
                    ["Alpha"] = 4,
                }
            ),
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void NestedJsonArraysWithObjects()
    {
        var obj = new JsonObject
        {
            ["Matrix"] = new JsonArray(
                new JsonArray(
                    new JsonObject { ["B"] = 1, ["A"] = 2 }
                )
            ),
        };

        obj.ShouldMatchSnapshot();
    }

    // --- Mixed with regular objects ---

    [Fact]
    public void JsonObjectInsideRegularDictionary()
    {
        var dict = new Dictionary<string, JsonObject>
        {
            ["Second"] = new JsonObject { ["Z"] = 1, ["A"] = 2 },
            ["First"] = new JsonObject { ["Y"] = 3, ["B"] = 4 },
        };

        JestAssert.ShouldMatchSnapshot(dict);
    }

    [Fact]
    public void JsonObjectAsPropertyOfPoco()
    {
        var wrapper = new JsonObjectWrapper
        {
            Name = "test",
            Data = new JsonObject
            {
                ["Zebra"] = 1,
                ["Apple"] = 2,
            },
        };

        JestAssert.ShouldMatchSnapshot(wrapper);
    }

    // --- Circular reference ---

    [Fact]
    public void CircularJsonObjectThrowsWhenAddingSelf()
    {
        var obj = new JsonObject { ["Name"] = "root" };

        Assert.ThrowsAny<Exception>(() =>
        {
            obj["Self"] = obj;
        });
    }

    // --- Large / stress ---

    [Fact]
    public void LargeNumberOfKeys()
    {
        // Keys inserted in reverse order — sorted alphabetically regardless of insertion order
        var obj = new JsonObject();
        for (var i = 99; i >= 0; i--)
        {
            obj[$"Key{i:D3}"] = i;
        }

        obj.ShouldMatchSnapshot();
    }

    // --- JsonObject with various JsonValue types ---

    [Fact]
    public void JsonObjectWithLargeNumbers()
    {
        var obj = new JsonObject
        {
            ["MaxLong"] = long.MaxValue,
            ["MinLong"] = long.MinValue,
            ["Double"] = 1.7976931348623157E+308,
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonObjectWithEmptyNestedStructures()
    {
        var obj = new JsonObject
        {
            ["EmptyArray"] = new JsonArray(),
            ["EmptyObject"] = new JsonObject(),
            ["NonEmpty"] = "value",
        };

        obj.ShouldMatchSnapshot();
    }

    [Fact]
    public void JsonObjectWithMixedArrayContent()
    {
        var obj = new JsonObject
        {
            ["Mixed"] = new JsonArray(
                1,
                "two",
                true,
                (JsonNode?)null,
                new JsonObject { ["B"] = 2, ["A"] = 1 }
            ),
        };

        obj.ShouldMatchSnapshot();
    }

    private class JsonObjectWrapper
    {
        public string? Name { get; set; }
        public JsonObject? Data { get; set; }
    }
}
