# Pre-Serializer Extension Point

## Problem

JestDotnet 2.0+ uses System.Text.Json (STJ) for serialization. When users pass objects from external libraries (e.g., Newtonsoft.Json's `JObject`, `JArray`, `JToken`) to `ShouldMatchSnapshot()`, STJ does not know how to serialize them, producing incorrect output such as empty arrays.

This is not limited to Newtonsoft. Any library with custom container types that STJ cannot introspect will have the same problem.

## Solution

Add a type-based pre-serializer extension point to `SnapshotSettings`. Users register a function that converts a specific type to a JSON string before STJ processes it.

## API

### Registration

```csharp
// Register a pre-serializer for a specific type
SnapshotSettings.AddPreSerializer<JObject>(obj => obj.ToString());
SnapshotSettings.AddPreSerializer<JArray>(obj => obj.ToString());

// Clear all pre-serializers (e.g., for test cleanup)
SnapshotSettings.ClearPreSerializers();
```

### Method Signatures

```csharp
// In SnapshotSettings
public static void AddPreSerializer<T>(Func<T, string> serializer);
public static void ClearPreSerializers();
```

## Serialization Pipeline Change

Current pipeline in `Serializer.Serialize(object obj)`:
1. Get `JsonSerializerOptions` from `SnapshotSettings.CreateSerializerOptions()`
2. Serialize `obj` with `JsonSerializer.Serialize(writer, obj, options)`
3. Return UTF-8 string

New pipeline:
1. Check if `obj`'s runtime type has a registered pre-serializer
2. **If match found:** call the pre-serializer to get a JSON string, parse it with `JsonNode.Parse()`, then serialize the `JsonNode` through STJ with normal options (indentation, encoding, etc.)
3. **If no match:** current behavior unchanged (serialize directly with STJ)

## Behavior Details

- **Type matching:** Exact runtime type match via `obj.GetType()`. No base type or interface walking.
- **Key ordering:** Preserved as-is from the pre-serializer output. This is consistent with how STJ's own `JsonNode` types behave in the current codebase (insertion order, not alphabetically sorted).
- **Re-serialization through STJ:** The parsed `JsonNode` is serialized using the configured `JsonSerializerOptions`, ensuring consistent formatting (indentation, encoding, line endings) regardless of what the pre-serializer produces.
- **Thread safety:** The pre-serializer dictionary is static mutable state, same as the existing `SnapshotSettings` fields. No additional thread-safety guarantees beyond what already exists.

## Storage

`SnapshotSettings` gets a new private static field:

```csharp
private static readonly Dictionary<Type, Func<object, string>> PreSerializers = new();
```

`AddPreSerializer<T>` wraps the typed `Func<T, string>` into a `Func<object, string>` with a cast internally.

## Files Changed

- `JestDotnet/Core/Settings/SnapshotSettings.cs` — add `PreSerializers` dictionary, `AddPreSerializer<T>()`, `ClearPreSerializers()`, and a method to look up and invoke a pre-serializer
- `JestDotnet/Core/Serializer.cs` — check for pre-serializer before STJ serialization; if found, call it, parse result as `JsonNode`, serialize the node

## No New Dependencies

This is purely a hook. Users bring their own types and conversion logic. JestDotnet has no compile-time or runtime dependency on Newtonsoft.Json or any other library.

## Example Usage

```csharp
// In test setup or assembly initializer
SnapshotSettings.AddPreSerializer<JObject>(obj => obj.ToString());
SnapshotSettings.AddPreSerializer<JArray>(obj => obj.ToString());

// Then in tests, this just works:
var json = JObject.Parse("{\"name\": \"Alice\", \"age\": 30}");
json.ShouldMatchSnapshot();
```
