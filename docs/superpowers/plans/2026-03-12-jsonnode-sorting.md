# JsonNode/JsonElement Alphabetical Sorting Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ensure all JSON output from `Serializer.Serialize` has alphabetically sorted object keys, regardless of whether the input is a POCO, JsonNode, JsonElement, or comes through a PreSerializer.

**Architecture:** Add a `SortJsonNode` method to `Serializer.cs` that recursively sorts `JsonObject` properties alphabetically. Apply it as a universal post-processing step after initial serialization — parse the JSON string to `JsonNode`, sort, re-serialize. This covers all code paths uniformly.

**Tech Stack:** C# 14, .NET 10, System.Text.Json, xUnit

**Spec:** `docs/superpowers/specs/2026-03-12-jsonnode-sorting-design.md`

---

## Chunk 1: Core Implementation and Existing Test Fixes

### Task 1: Add `SortJsonNode` method with unit test

**Files:**
- Modify: `JestDotnet/JestDotnet/Core/Serializer.cs`
- Create: `JestDotnet/XUnitTests/SortJsonNodeTests.cs`

- [ ] **Step 1: Write failing test for `SortJsonNode`**

Create `JestDotnet/XUnitTests/SortJsonNodeTests.cs`:

```csharp
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
```

Note: `SortJsonNode` must be `internal` with `[assembly: InternalsVisibleTo("XUnitTests")]` or made accessible. Check if `InternalsVisibleTo` is already configured — if not, add it to `JestDotnet.csproj` or an `AssemblyInfo.cs`.

- [ ] **Step 2: Run tests to verify they fail**

Run: `cd JestDotnet && dotnet test --filter "SortJsonNodeTests" -v n`
Expected: Build error — `SortJsonNode` does not exist yet.

- [ ] **Step 3: Implement `SortJsonNode`**

In `JestDotnet/JestDotnet/Core/Serializer.cs`, add the method:

```csharp
internal static void SortJsonNode(JsonNode? node)
{
    switch (node)
    {
        case JsonObject obj:
            var sorted = obj.OrderBy(p => p.Key, StringComparer.Ordinal).ToList();
            obj.Clear();
            foreach (var (key, value) in sorted)
            {
                obj.Add(key, value);
                SortJsonNode(value);
            }
            break;
        case JsonArray arr:
            foreach (var element in arr)
            {
                SortJsonNode(element);
            }
            break;
    }
}
```

Add required using at top of file if not present:

```csharp
using System;
using System.Linq;
```

- [ ] **Step 4: Ensure `InternalsVisibleTo` is configured**

Check if `InternalsVisibleTo` already exists for the test project. If not, add to `JestDotnet/JestDotnet/JestDotnet.csproj`:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="XUnitTests" />
</ItemGroup>
```

- [ ] **Step 5: Run tests to verify they pass**

Run: `cd JestDotnet && dotnet test --filter "SortJsonNodeTests" -v n`
Expected: All 5 tests PASS.

- [ ] **Step 6: Commit**

```bash
git add JestDotnet/JestDotnet/Core/Serializer.cs JestDotnet/XUnitTests/SortJsonNodeTests.cs JestDotnet/JestDotnet/JestDotnet.csproj
git commit -m "feat: add SortJsonNode method for recursive alphabetical sorting"
```

---

### Task 2: Integrate `SortJsonNode` into `Serializer.Serialize`

**Files:**
- Modify: `JestDotnet/JestDotnet/Core/Serializer.cs:11-34`

- [ ] **Step 1: Write a failing test that proves the bug exists**

Create a test in `JestDotnet/XUnitTests/SortJsonNodeTests.cs` that uses the public API:

```csharp
[Fact]
public void SerializeSortsJsonObjectKeysAlphabetically()
{
    var obj = new JsonObject
    {
        ["zebra"] = 1,
        ["apple"] = 2,
        ["mango"] = 3,
    };

    var result = Serializer.Serialize(obj);

    Assert.Contains("\"apple\"", result);
    Assert.Contains("\"mango\"", result);
    Assert.Contains("\"zebra\"", result);

    // Verify alphabetical order
    var appleIdx = result.IndexOf("\"apple\"");
    var mangoIdx = result.IndexOf("\"mango\"");
    var zebraIdx = result.IndexOf("\"zebra\"");
    Assert.True(appleIdx < mangoIdx);
    Assert.True(mangoIdx < zebraIdx);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd JestDotnet && dotnet test --filter "SerializeSortsJsonObjectKeysAlphabetically" -v n`
Expected: FAIL — keys are in insertion order (zebra, apple, mango).

- [ ] **Step 3: Modify `Serializer.Serialize` to add universal post-sort**

Replace the `Serialize` method body in `JestDotnet/JestDotnet/Core/Serializer.cs`:

```csharp
internal static string Serialize(object? obj)
{
    var options = SnapshotSettings.CreateSerializerOptions();
    var writerOptions = new JsonWriterOptions
    {
        Indented = options.WriteIndented,
        NewLine = SnapshotSettings.NewLine,
        Encoder = options.Encoder,
    };

    using var stream = new MemoryStream();
    using var writer = new Utf8JsonWriter(stream, writerOptions);
    if (obj is not null && SnapshotSettings.TryGetPreSerializer(obj.GetType(), out var preSerializer))
    {
        var json = preSerializer!(obj);
        var node = JsonNode.Parse(json);
        JsonSerializer.Serialize(writer, node, options);
    }
    else
    {
        JsonSerializer.Serialize(writer, obj, options);
    }

    writer.Flush();
    var result = Encoding.UTF8.GetString(stream.ToArray());

    var sorted = JsonNode.Parse(result);
    SortJsonNode(sorted);

    using var sortedStream = new MemoryStream();
    using var sortedWriter = new Utf8JsonWriter(sortedStream, writerOptions);
    JsonSerializer.Serialize(sortedWriter, sorted, options);
    sortedWriter.Flush();
    return Encoding.UTF8.GetString(sortedStream.ToArray());
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "SerializeSortsJsonObjectKeysAlphabetically" -v n`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add JestDotnet/JestDotnet/Core/Serializer.cs JestDotnet/XUnitTests/SortJsonNodeTests.cs
git commit -m "feat: integrate universal post-sort into Serializer.Serialize"
```

---

### Task 3: Update existing snapshots and fix existing tests

**Files:**
- Modify: `JestDotnet/XUnitTests/JsonObjectBugTest.cs`
- Modify: `JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs`
- Modify: `JestDotnet/XUnitTests/PreSerializerTests.cs`
- Multiple `.snap` files regenerated

- [ ] **Step 1: Run all tests to see which fail**

Run: `cd JestDotnet && dotnet test -v n`
Expected: Multiple snapshot mismatches. Note which tests fail.

- [ ] **Step 2: Rename test methods in `JsonObjectBugTest.cs`**

Rename in `JestDotnet/XUnitTests/JsonObjectBugTest.cs`:
- `JsonObjectKeysUseInsertionOrder` → `JsonObjectKeysShouldBeSortedAlphabetically`
- `NestedJsonObjectKeysUseInsertionOrder` → `NestedJsonObjectKeysShouldBeSortedAlphabetically`

Delete the old snapshot files (the renamed tests will create new ones):
- `JestDotnet/XUnitTests/__snapshots__/JsonObjectBugTestJsonObjectKeysUseInsertionOrder.snap`
- `JestDotnet/XUnitTests/__snapshots__/JsonObjectBugTestNestedJsonObjectKeysUseInsertionOrder.snap`

- [ ] **Step 3: Rename test method and update inline snapshot in `PreSerializerTests.cs`**

In `JestDotnet/XUnitTests/PreSerializerTests.cs`:
- Rename `PreSerializerShouldPreserveKeyOrder` → `PreSerializerShouldSortKeysAlphabetically`
- Update the inline snapshot literal so `"aAge"` comes before `"zName"`:

```csharp
[Fact]
public void PreSerializerShouldSortKeysAlphabetically()
{
    SnapshotSettings.AddPreSerializer<CustomSerializable>(
        obj => $$"""{"zName": "{{obj.Name}}", "aAge": {{obj.Age}}}""");

    try
    {
        var obj = new CustomSerializable { Name = "Bob", Age = 25 };

        obj.ShouldMatchInlineSnapshot("""
                                      {
                                        "aAge": 25,
                                        "zName": "Bob"
                                      }
                                      """);
    }
    finally
    {
        SnapshotSettings.ClearPreSerializers();
    }
}
```

- [ ] **Step 4: Update comment in `JsonObjectEdgeCaseTests.cs`**

In the `LargeNumberOfKeys` test, update the comment that says "to verify insertion-order preservation (not sorted)" to "keys are sorted alphabetically regardless of insertion order".

- [ ] **Step 5: Regenerate all snapshots**

Run: `cd JestDotnet && UPDATE=true dotnet test -v n`
Expected: All tests PASS. Snapshots regenerated with sorted keys.

- [ ] **Step 6: Verify all tests pass without UPDATE**

Run: `cd JestDotnet && dotnet test -v n`
Expected: All tests PASS.

- [ ] **Step 7: Commit**

```bash
git add JestDotnet/XUnitTests/
git commit -m "fix: update tests and snapshots for alphabetical JsonNode sorting"
```

---

## Chunk 2: New Tests for Previously Broken Paths

### Task 4: Add test for `JsonElement` passed directly

**Files:**
- Modify: `JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs`

- [ ] **Step 1: Write the test**

Add to `JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs`:

```csharp
[Fact]
public void JsonElementShouldBeSortedAlphabetically()
{
    using var doc = JsonDocument.Parse("""{"zebra":1,"apple":2,"mango":3}""");
    var element = doc.RootElement;

    element.ShouldMatchInlineSnapshot("""
                                      {
                                        "apple": 2,
                                        "mango": 3,
                                        "zebra": 1
                                      }
                                      """);
}
```

Ensure `using System.Text.Json;` is present at the top of the file.

- [ ] **Step 2: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "JsonElementShouldBeSortedAlphabetically" -v n`
Expected: PASS (the universal post-sort handles this).

- [ ] **Step 3: Commit**

```bash
git add JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs
git commit -m "test: add JsonElement alphabetical sorting test"
```

---

### Task 5: Add test for POCO with nested `JsonObject` property

**Files:**
- Modify: `JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs`

- [ ] **Step 1: Write the test**

The `JsonObjectAsPropertyOfPoco` test already exists but its snapshot will now have sorted inner keys. Add an explicit test that verifies the nested `JsonObject` keys are sorted:

```csharp
[Fact]
public void PocoWithJsonObjectPropertySortsNestedKeys()
{
    var wrapper = new JsonObjectWrapper
    {
        Name = "test",
        Data = new JsonObject
        {
            ["zebra"] = 1,
            ["apple"] = 2,
            ["mango"] = 3,
        },
    };

    wrapper.ShouldMatchInlineSnapshot("""
                                      {
                                        "Data": {
                                          "apple": 2,
                                          "mango": 3,
                                          "zebra": 1
                                        },
                                        "Name": "test"
                                      }
                                      """);
}
```

This uses the existing `JsonObjectWrapper` class already defined in `JsonObjectEdgeCaseTests.cs`.

- [ ] **Step 2: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PocoWithJsonObjectPropertySortsNestedKeys" -v n`
Expected: PASS.

- [ ] **Step 3: Commit**

```bash
git add JestDotnet/XUnitTests/JsonObjectEdgeCaseTests.cs
git commit -m "test: add POCO with nested JsonObject sorting test"
```

---

### Task 6: Final verification

- [ ] **Step 1: Run full test suite**

Run: `cd JestDotnet && dotnet test -v n`
Expected: All tests PASS.

- [ ] **Step 2: Run build with no warnings**

Run: `cd JestDotnet && dotnet build --no-restore`
Expected: Build succeeded. 0 Warning(s). 0 Error(s). (`TreatWarningsAsErrors` is enabled.)

- [ ] **Step 3: Commit any remaining changes**

If any files were missed, stage and commit them.
