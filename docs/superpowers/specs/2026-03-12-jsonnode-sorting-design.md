# JsonNode/JsonElement Alphabetical Sorting

## Problem

`AlphabeticalSortModifier.SortProperties` only sorts POCOs (`JsonTypeInfoKind.Object`). When objects pass through `AddPreSerializer`, they become a `JsonNode` via `JsonNode.Parse(json)`, which bypasses the modifier entirely. The same issue affects any `JsonElement` or `JsonNode` passed directly to `ShouldMatchSnapshot()`, and `JsonNode` values nested inside POCOs or dictionaries (e.g., a POCO with a `JsonObject` property, or a `Dictionary<string, JsonObject>`).

This means snapshot output for these types depends on property insertion order, which is fragile and inconsistent with the POCO behavior.

## Approach

Sort the final serialized JSON output universally. After `Serializer.Serialize` produces its JSON string, parse it to a `JsonNode`, recursively sort all `JsonObject` properties alphabetically, and re-serialize. This covers every code path uniformly — POCOs, PreSerializer output, direct JsonNode/JsonElement, and nested JsonNode inside POCOs or dictionaries.

The double-serialize for already-sorted POCOs is negligible in a snapshot testing context.

## Design

### New method: `SortJsonNode(JsonNode?)`

Added to `Serializer.cs`. Recursively walks a `JsonNode` tree and reorders `JsonObject` properties alphabetically.

**Logic:**
- **JsonObject**: Extract all key-value pairs, remove them, re-add in sorted order (using `StringComparer.Ordinal` to match `AlphabeticalSortModifier`). Recurse into each value.
- **JsonArray**: Recurse into each element.
- **Value nodes / null**: No-op.

### Modifications to `Serializer.Serialize`

Replace the current serialization logic with a single post-processing step:

```csharp
internal static string Serialize(object? obj)
{
    var options = SnapshotSettings.CreateSerializerOptions();
    using var stream = new MemoryStream();
    using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
    {
        Indented = options.WriteIndented,
        NewLine = SnapshotSettings.NewLine,
        Encoder = options.Encoder,
    });

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

    // Universally sort all JsonObject properties alphabetically.
    // This covers POCOs with nested JsonNodes, PreSerializer output,
    // direct JsonNode/JsonElement input, and all other paths.
    var sorted = JsonNode.Parse(result);
    SortJsonNode(sorted);

    using var sortedStream = new MemoryStream();
    using var sortedWriter = new Utf8JsonWriter(sortedStream, new JsonWriterOptions
    {
        Indented = options.WriteIndented,
        NewLine = SnapshotSettings.NewLine,
        Encoder = options.Encoder,
    });
    JsonSerializer.Serialize(sortedWriter, sorted, options);
    sortedWriter.Flush();
    return Encoding.UTF8.GetString(sortedStream.ToArray());
}
```

This avoids mutating any caller-owned `JsonNode` — we always work on a freshly parsed copy.

### Affected files

- `JestDotnet/JestDotnet/Core/Serializer.cs` — add `SortJsonNode`, modify `Serialize`

### Test changes

**Existing snapshots that will change** (insertion order → alphabetical):
- `JsonObjectBugTestJsonObjectKeysUseInsertionOrder.snap`
- `JsonObjectBugTestNestedJsonObjectKeysUseInsertionOrder.snap`
- `JsonObjectEdgeCaseTestsJsonObjectAsPropertyOfPoco.snap`
- `JsonObjectEdgeCaseTestsJsonObjectInsideRegularDictionary.snap`
- `JsonObjectEdgeCaseTestsLargeNumberOfKeys.snap`
- `PreSerializerShouldPreserveKeyOrder` inline snapshot

**Test renames needed:**
- `PreSerializerShouldPreserveKeyOrder` → `PreSerializerShouldSortKeysAlphabetically` (behavior changed)
- Update comment in `JsonObjectEdgeCaseTests.cs` `LargeNumberOfKeys` test that says "to verify insertion-order preservation (not sorted)"

**New tests:**
- `JsonElement` passed directly to `ShouldMatchSnapshot` with unsorted keys → verify sorted output
- POCO containing `JsonObject` property → verify nested JsonObject keys are sorted
