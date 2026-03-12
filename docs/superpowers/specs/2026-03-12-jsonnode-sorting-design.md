# JsonNode/JsonElement Alphabetical Sorting

## Problem

`AlphabeticalSortModifier.SortProperties` only sorts POCOs (`JsonTypeInfoKind.Object`). When objects pass through `AddPreSerializer`, they become a `JsonNode` via `JsonNode.Parse(json)`, which bypasses the modifier entirely. The same issue affects any `JsonElement` or `JsonNode` passed directly to `ShouldMatchSnapshot()`, and `JsonNode` values nested inside POCOs or dictionaries (e.g., a POCO with a `JsonObject` property, or a `Dictionary<string, JsonObject>`).

This means snapshot output for these types depends on property insertion order, which is fragile and inconsistent with the POCO behavior.

## Approach

Sort the final serialized JSON output universally. After `Serializer.Serialize` produces its JSON string, parse it to a `JsonNode`, recursively sort all `JsonObject` properties alphabetically, and re-serialize. This covers every code path uniformly — POCOs, PreSerializer output, direct JsonNode/JsonElement, and nested JsonNode inside POCOs or dictionaries.

The double-serialize for already-sorted POCOs is negligible in a snapshot testing context.

`AlphabeticalSortModifier.SortProperties` is retained in the default `JsonSerializerOptions` for backward compatibility — callers who access the options directly still get sorted POCOs. It becomes redundant for the `Serialize` path but harmless.

## Design

### New method: `SortJsonNode(JsonNode?)`

Added to `Serializer.cs`. Recursively walks a `JsonNode` tree and reorders `JsonObject` properties alphabetically.

**Logic:**
- **null**: No-op. This handles JSON `null` literals (e.g., `Serialize(null)` produces `"null"`, which `JsonNode.Parse` returns as `null`). The subsequent `JsonSerializer.Serialize` call with `null` correctly outputs `null`.
- **JsonObject**: Extract all key-value pairs, remove them, re-add in sorted order (using `StringComparer.Ordinal` to match `AlphabeticalSortModifier`). Recurse into each value.
- **JsonArray**: Recurse into each element.
- **Value nodes**: No-op.

### Modifications to `Serializer.Serialize`

The existing serialization (both the PreSerializer and default paths) runs first to produce a JSON string. Then a universal post-sort step parses, sorts, and re-serializes:

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

Note: The PreSerializer branch could be collapsed (just `JsonSerializer.Serialize(writer, obj, options)` for everything, since the post-sort handles all ordering). However, the PreSerializer branch must remain because it applies custom serialization logic — it converts the object to a JSON string via the user's function, then parses to `JsonNode` for re-serialization with the library's formatting options.

This avoids mutating any caller-owned `JsonNode` — we always work on a freshly parsed copy.

### Affected files

- `JestDotnet/JestDotnet/Core/Serializer.cs` — add `SortJsonNode`, modify `Serialize`

### Test changes

**Existing snapshots**: Many snapshots contain insertion-order keys that will now be sorted alphabetically. Run `UPDATE=true dotnet test` to regenerate all snapshots, then review the diffs.

**Test renames needed:**
- `PreSerializerShouldPreserveKeyOrder` → `PreSerializerShouldSortKeysAlphabetically` — update the inline snapshot literal to have `"aAge"` before `"zName"`
- `JsonObjectKeysUseInsertionOrder` → `JsonObjectKeysShouldBeSortedAlphabetically`
- `NestedJsonObjectKeysUseInsertionOrder` → `NestedJsonObjectKeysShouldBeSortedAlphabetically`
- Update comment in `JsonObjectEdgeCaseTests.cs` `LargeNumberOfKeys` test that says "to verify insertion-order preservation (not sorted)"

**New tests:**
- `JsonElement` passed directly to `ShouldMatchSnapshot` with unsorted keys → verify sorted output
- POCO containing `JsonObject` property → verify nested JsonObject keys are sorted

**Inline snapshots**: After this change, inline snapshot strings must use alphabetically sorted keys. The `ShouldMatchInlineSnapshot` API compares against the sorted serialized output.
