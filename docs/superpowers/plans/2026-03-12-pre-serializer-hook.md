# Pre-Serializer Extension Point Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a type-based pre-serializer hook to `SnapshotSettings` so users can register custom serializers for types STJ cannot handle (e.g., Newtonsoft `JObject`).

**Architecture:** `SnapshotSettings` stores a `Dictionary<Type, Func<object, string>>` of pre-serializers. `Serializer.Serialize` checks this dictionary before STJ serialization; if a match is found, the pre-serializer produces a JSON string which is parsed into a `JsonNode` and re-serialized through STJ for consistent formatting.

**Tech Stack:** C# 14, .NET 10, System.Text.Json, xUnit 2.9.3

**Spec:** `docs/superpowers/specs/2026-03-12-pre-serializer-hook-design.md`

---

## Chunk 1: Core Implementation

### Task 1: Add pre-serializer storage and API to SnapshotSettings

**Files:**
- Modify: `JestDotnet/JestDotnet/Core/Settings/SnapshotSettings.cs`
- Test: `JestDotnet/XUnitTests/PreSerializerTests.cs`

- [ ] **Step 1: Write the failing test — AddPreSerializer registers and ClearPreSerializers clears**

Create `JestDotnet/XUnitTests/PreSerializerTests.cs`:

```csharp
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ShouldSerializeCustomTypeViaPreSerializer"`
Expected: FAIL — `AddPreSerializer` method does not exist.

- [ ] **Step 3: Add pre-serializer dictionary and public methods to SnapshotSettings**

In `JestDotnet/JestDotnet/Core/Settings/SnapshotSettings.cs`, add after the `CreateDiffOptions` field (after line 93):

```csharp
    private static readonly Dictionary<Type, Func<object, string>> PreSerializers = new();

    public static void AddPreSerializer<T>(Func<T, string> serializer)
    {
        PreSerializers[typeof(T)] = obj => serializer((T)obj);
    }

    public static void ClearPreSerializers()
    {
        PreSerializers.Clear();
    }

    internal static bool TryGetPreSerializer(Type type, out Func<object, string>? serializer)
    {
        return PreSerializers.TryGetValue(type, out serializer);
    }
```

Add `using System.Collections.Generic;` to the top if not already present.

- [ ] **Step 4: Run test to verify it still fails (method exists but Serializer doesn't use it yet)**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ShouldSerializeCustomTypeViaPreSerializer"`
Expected: FAIL — snapshot mismatch (STJ serializes `CustomSerializable` as a POCO with alphabetically sorted keys, not using the pre-serializer).

- [ ] **Step 5: Commit**

```bash
cd JestDotnet && git add JestDotnet/Core/Settings/SnapshotSettings.cs XUnitTests/PreSerializerTests.cs && git commit -m "feat: add pre-serializer API to SnapshotSettings (not yet wired)"
```

### Task 2: Wire pre-serializer into Serializer

**Files:**
- Modify: `JestDotnet/JestDotnet/Core/Serializer.cs`
- Test: `JestDotnet/XUnitTests/PreSerializerTests.cs`

- [ ] **Step 1: Modify Serializer.Serialize to check for pre-serializer**

Replace the body of `Serialize` in `JestDotnet/JestDotnet/Core/Serializer.cs`:

```csharp
    internal static string Serialize(object obj)
    {
        var options = SnapshotSettings.CreateSerializerOptions();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = options.WriteIndented,
            NewLine = SnapshotSettings.NewLine,
            Encoder = options.Encoder,
        });

        if (SnapshotSettings.TryGetPreSerializer(obj.GetType(), out var preSerializer))
        {
            var json = preSerializer!(obj);
            var node = System.Text.Json.Nodes.JsonNode.Parse(json);
            JsonSerializer.Serialize(writer, node, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, obj, options);
        }

        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
```

Add `using System.Text.Json.Nodes;` if not using the fully qualified name.

- [ ] **Step 2: Run the test from Task 1 to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ShouldSerializeCustomTypeViaPreSerializer"`
Expected: PASS

- [ ] **Step 3: Run all tests to verify no regressions**

Run: `cd JestDotnet && dotnet test`
Expected: All tests PASS

- [ ] **Step 4: Commit**

```bash
cd JestDotnet && git add JestDotnet/Core/Serializer.cs && git commit -m "feat: wire pre-serializer hook into Serializer pipeline"
```

### Task 3: Add tests for edge cases

**Files:**
- Modify: `JestDotnet/XUnitTests/PreSerializerTests.cs`

- [ ] **Step 1: Add test — pre-serializer preserves key order (not alphabetical)**

```csharp
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
```

- [ ] **Step 2: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.PreSerializerShouldPreserveKeyOrder"`
Expected: PASS — keys stay in `zName, aAge` order, not sorted alphabetically.

- [ ] **Step 3: Add test — objects without pre-serializer still work normally**

```csharp
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
```

- [ ] **Step 4: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ObjectsWithoutPreSerializerUseDefaultSerialization"`
Expected: PASS — anonymous object is alphabetically sorted by default STJ pipeline.

- [ ] **Step 5: Add test — ClearPreSerializers removes registrations**

```csharp
    [Fact]
    public void ClearPreSerializersShouldRemoveAllRegistrations()
    {
        SnapshotSettings.AddPreSerializer<CustomSerializable>(obj => obj.ToJson());
        SnapshotSettings.ClearPreSerializers();

        // Without pre-serializer, STJ serializes as a POCO (alphabetical keys)
        var obj = new CustomSerializable("Alice", 30);
        JestAssert.ShouldMatchInlineSnapshot(obj, """
            {
              "Age": 30,
              "Name": "Alice"
            }
            """);
    }
```

- [ ] **Step 6: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ClearPreSerializersShouldRemoveAllRegistrations"`
Expected: PASS

- [ ] **Step 7: Add test — ShouldMatchObject works with pre-serializer on both sides**

```csharp
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
```

- [ ] **Step 8: Run test to verify it passes**

Run: `cd JestDotnet && dotnet test --filter "PreSerializerTests.ShouldMatchObjectWorksWithPreSerializer"`
Expected: PASS

- [ ] **Step 9: Run all tests**

Run: `cd JestDotnet && dotnet test`
Expected: All tests PASS

- [ ] **Step 10: Commit**

```bash
cd JestDotnet && git add XUnitTests/PreSerializerTests.cs && git commit -m "test: add edge case tests for pre-serializer hook"
```
