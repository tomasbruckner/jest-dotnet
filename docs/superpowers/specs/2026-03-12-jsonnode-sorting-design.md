# JsonNode/JsonElement Alphabetical Sorting

## Problem

`AlphabeticalSortModifier.SortProperties` only sorts POCOs (`JsonTypeInfoKind.Object`). When objects pass through `AddPreSerializer`, they become a `JsonNode` via `JsonNode.Parse(json)`, which bypasses the modifier entirely. The same issue affects any `JsonElement` or `JsonNode` passed directly to `ShouldMatchSnapshot()`.

This means snapshot output for these types depends on property insertion order, which is fragile and inconsistent with the POCO behavior.

## Approach

Add a recursive `SortJsonNode` method to `Serializer.cs` and apply it before serialization in all non-POCO paths.

## Design

### New method: `SortJsonNode(JsonNode?)`

Added to `Serializer.cs`. Recursively walks a `JsonNode` tree and reorders `JsonObject` properties alphabetically.

**Logic:**
- **JsonObject**: Extract all key-value pairs, remove them, re-add in sorted order (using `StringComparer.Ordinal` to match `AlphabeticalSortModifier`). Recurse into each value.
- **JsonArray**: Recurse into each element.
- **Value nodes / null**: No-op.

### Modifications to `Serializer.Serialize`

Two changes in the `Serialize` method:

1. **PreSerializer path** — after `JsonNode.Parse(json)`, call `SortJsonNode(node)` before serializing:
   ```csharp
   var json = preSerializer!(obj);
   var node = JsonNode.Parse(json);
   SortJsonNode(node);
   JsonSerializer.Serialize(writer, node, options);
   ```

2. **Direct JsonNode/JsonElement path** — detect when `obj` is a `JsonNode` or `JsonElement`, convert to `JsonNode` if needed, sort, and serialize via the node path:
   ```csharp
   if (obj is JsonElement element)
   {
       var node = JsonNode.Parse(element.GetRawText());
       SortJsonNode(node);
       JsonSerializer.Serialize(writer, node, options);
   }
   else if (obj is JsonNode jsonNode)
   {
       SortJsonNode(jsonNode);
       JsonSerializer.Serialize(writer, jsonNode, options);
   }
   else
   {
       JsonSerializer.Serialize(writer, obj, options);
   }
   ```

### Affected files

- `JestDotnet/JestDotnet/Core/Serializer.cs` — add `SortJsonNode`, modify `Serialize`

### Test plan

- Update existing snapshots that relied on insertion order (e.g., `JsonObjectKeysUseInsertionOrder`, `PreSerializerShouldPreserveKeyOrder`)
- New tests:
  - Sorted `JsonObject` passed directly
  - Sorted nested `JsonObject`
  - Sorted `JsonElement`
  - Sorted PreSerializer output with unsorted keys
