# Architecture

## Overview

JestDotnet is a snapshot testing library for .NET. It serializes objects to JSON, saves the output to `.snap` files, and compares against them on subsequent test runs.

## Project Structure

```
JestDotnet/
├── JestAssert.cs                    # Public API — static methods
├── JestDotnetExtensions.cs          # Public API — extension methods on object
└── Core/
    ├── Serializer.cs                # JSON serialization via System.Text.Json
    ├── SnapshotResolver.cs          # Snapshot file I/O and path resolution
    ├── SnapshotComparer.cs          # JSON diff comparison
    ├── SnapshotUpdater.cs           # Create/update/fail orchestration
    ├── Settings/
    │   ├── AlphabeticalSortModifier.cs       # Default property sorting modifier
    │   └── SnapshotSettings.cs              # Global configuration
    └── Exceptions/
        ├── SnapshotDoesNotExist.cs   # Thrown when snapshot missing in CI
        └── SnapshotMismatch.cs       # Thrown when snapshot doesn't match
```

## Public API

Two equivalent entry points provide the same three methods:

| Method | Description |
|--------|-------------|
| `ShouldMatchSnapshot` | Compare object against a `.snap` file (created automatically on first run) |
| `ShouldMatchInlineSnapshot` | Compare object against a JSON string passed directly in code |
| `ShouldMatchObject` | Compare two objects against each other |

`JestAssert` exposes them as static methods, `JestDotnetExtensions` as extension methods on `object`.

## Snapshot Lifecycle

```
Test calls ShouldMatchSnapshot(object)
  │
  ├─ Resolve path: __snapshots__/{TestFile}{MethodName}{Hint}.snap
  │
  ├─ Read snapshot file
  │   │
  │   ├─ File missing?
  │   │   ├─ CI=true  → throw SnapshotDoesNotExist
  │   │   └─ else     → serialize object, create file
  │   │
  │   └─ File exists → compare
  │       │
  │       ├─ Match    → test passes
  │       └─ Mismatch
  │           ├─ UPDATE=true → overwrite snapshot
  │           └─ else        → throw SnapshotMismatch (with JSON diff)
```

## Core Components

### Serializer

Converts objects to indented JSON using `System.Text.Json`. Uses `Utf8JsonWriter` for control over formatting (line endings, Unicode encoding). Properties are sorted alphabetically via `AlphabeticalSortModifier` using ordinal string comparison for culture-independent output. Circular references are handled via `ReferenceHandler.IgnoreCycles` (cyclic references written as `null`). Configuration comes from `SnapshotSettings.CreateSerializerOptions`.

### SnapshotResolver

Handles file I/O. Reads snapshot files (returns empty string if missing), writes them (creates directories as needed), and derives file paths from test metadata using `[CallerFilePath]` and `[CallerMemberName]` attributes.

**Path formula:** `{test file directory}/__snapshots__/{TestFileName}{MethodName}{Hint}.snap`

### SnapshotComparer

Parses both expected and actual JSON into `JsonNode` objects, then diffs them using `SystemTextJson.JsonDiffPatch`. Returns a tuple of `(bool IsValid, string? Message)` where the message contains the JSON patch showing differences.

### SnapshotUpdater

Decides whether to create, update, or fail based on environment variables:

| Variable | Value | Effect |
|----------|-------|--------|
| `CI` | `true` | Prevents snapshot creation — test fails if snapshot is missing |
| `UPDATE` | `true` | Overwrites mismatched snapshots instead of failing |

## Configuration

All settings live in `SnapshotSettings` as static properties. Each has a default and can be overridden:

| Setting | Type | Default | Purpose |
|---------|------|---------|---------|
| `SnapshotExtension` | `string` | `"snap"` | Snapshot file extension |
| `SnapshotDirectory` | `string` | `"__snapshots__"` | Directory name for snapshots |
| `CreatePath` | `Func<..., string>` | Derives from test context | Custom path resolution |
| `CreateSerializerOptions` | `Func<JsonSerializerOptions>` | Indented, full Unicode | JSON serialization config |
| `CreateDiffOptions` | `Func<JsonDiffOptions?>` | `null` | Diff/comparison config (e.g., property exclusion) |
| `NewLine` | `string` | `"\n"` | Line ending in JSON output |

## Exceptions

JestDotnet can throw three categories of exceptions:

### JestDotnet exceptions

| Exception | When |
|-----------|------|
| `SnapshotDoesNotExist` | Snapshot file is missing and `CI=true` (prevents accidental snapshot creation in CI) |
| `SnapshotMismatch` | Serialized object differs from saved snapshot (and `UPDATE` is not `true`) |

### System.Text.Json exceptions

JestDotnet does not catch serialization errors — they bubble up directly from `System.Text.Json`:

| Exception | When | Example |
|-----------|------|---------|
| `JsonException` | Object graph exceeds max serialization depth (64) | 65+ levels of nested objects |
| `NotSupportedException` | Type has no compatible JSON converter | Serializing `IntPtr`, `Span<T>`, or other unsupported types |

### Circular references

Circular references are handled gracefully by default. `ReferenceHandler.IgnoreCycles` is enabled in the default serializer options — cyclic references are written as `null` instead of throwing. This applies to all cycle patterns (self-reference, mutual reference, cycles through lists and dictionaries).

```csharp
var node = new Node { Name = "root" };
node.Parent = node; // circular reference
node.ShouldMatchSnapshot(); // works — Parent is serialized as null
```

`JsonObject` (`System.Text.Json.Nodes`) prevents cycles at the API level — adding a node that already has a parent throws `InvalidOperationException`.

## Dependencies

- **System.Text.Json** — JSON serialization (built into .NET)
- **SystemTextJson.JsonDiffPatch** — semantic JSON comparison
