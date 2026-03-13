# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.4.0] - 2026-03-13

### Changed
- JSON encoder switched from `JavaScriptEncoder.Create(UnicodeRanges.All)` to `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` — HTML-sensitive characters (`<`, `>`, `&`) now appear as literals in snapshots instead of Unicode escape sequences (`\u003C`, `\u003E`, `\u0026`)

### Upgrade notes
- Snapshots containing HTML-sensitive characters will change. Run `UPDATE=true dotnet test` to regenerate snapshots after upgrading.

## [2.3.0] - 2026-03-13

### Fixed
- `JsonNode`, `JsonElement`, and pre-serializer output now have keys sorted alphabetically, matching the existing POCO behavior. Previously, these types preserved insertion order, producing inconsistent snapshots.
- `JsonNode` values nested inside POCOs or dictionaries are also sorted.

### Upgrade notes
- All snapshot files containing `JsonObject`, `JsonElement`, or pre-serializer output will change (keys sorted alphabetically instead of insertion order). Run `UPDATE=true dotnet test` to regenerate snapshots after upgrading.
- Inline snapshots (`ShouldMatchInlineSnapshot`) must be updated to use alphabetically sorted keys.

## [2.2.0] - 2026-03-12

### Added
- Pre-serializer extension point — register custom type-to-JSON converters for types that System.Text.Json cannot serialize. Use `SnapshotSettings.AddPreSerializer<T>()` to register and `SnapshotSettings.ClearPreSerializers()` to reset.

### Fixed
- Newtonsoft.Json types (`JObject`, `JArray`, `JToken`) now work correctly when using the pre-serializer hook — previously STJ produced empty arrays or incorrect output for these types

### Changed
- All public API methods (`ShouldMatchSnapshot`, `ShouldMatchInlineSnapshot`, `ShouldMatchObject`) now accept `null` without requiring the null-forgiving operator (`!`)

## [2.1.0] - 2026-03-11

### Changed
- `ReferenceHandler.IgnoreCycles` enabled by default — circular references are serialized as `null` instead of throwing `JsonException`

### Removed
- `SortedDictionaryConverterFactory` — removed custom dictionary key sorting converter. POCO properties are still sorted alphabetically via `AlphabeticalSortModifier`. As of v2.3.0, all JSON object keys (including `JsonObject` and `JsonElement`) are also sorted alphabetically.

### Upgrade notes
- Dictionary snapshots will change. Run `UPDATE=true dotnet test` to regenerate snapshots after upgrading.

## [2.0.1] - 2026-03-11

### Changed
- Properties and dictionary keys are now sorted alphabetically by default (ordinal comparison) for deterministic, culture-independent snapshots
- `AlphabeticalSortModifier` and `SortedDictionaryConverterFactory` shipped in the library
- Publish workflow now creates GitHub releases with attached `.nupkg` files

## [2.0.0] - 2026-03-11

### Changed
- **Serializer replaced:** Newtonsoft.Json removed in favor of System.Text.Json (STJ)
- **Target framework:** Minimum target changed from `net8.0` to `net10.0` only
- **Unicode in snapshots:** Non-ASCII characters (e.g., Cyrillic, CJK) are now rendered as literal UTF-8 instead of `\uXXXX` escape sequences, using `JavaScriptEncoder.Create(UnicodeRanges.All)`

### Removed
- `SnapshotSettings.CreateJsonSerializer` / `DefaultCreateJsonSerializer`
- `SnapshotSettings.CreateJTokenWriter` / `DefaultCreateJTokenWriter`
- `SnapshotSettings.CreateStringWriter` / `DefaultCreateStringWriter`
- `SnapshotSettings.CreateTextWriter` / `DefaultCreateTextWriter`
- `Newtonsoft.Json` package dependency

### Added
- `SnapshotSettings.CreateSerializerOptions` (`Func<JsonSerializerOptions>`) — single factory for all serialization configuration (default: `WriteIndented = true`)
- `SnapshotSettings.NewLine` (`string`) — controls line endings in JSON output (replaces `CreateStringWriter` customization)

### Migration guide
1. Update target framework to `net10.0`
2. Replace Newtonsoft serialization customization with `JsonSerializerOptions`:
   ```csharp
   // Before (Newtonsoft)
   SnapshotSettings.CreateJsonSerializer = () =>
   {
       var serializer = SnapshotSettings.DefaultCreateJsonSerializer();
       serializer.ContractResolver = new MyResolver();
       return serializer;
   };

   // After (STJ)
   SnapshotSettings.CreateSerializerOptions = () => new JsonSerializerOptions
   {
       WriteIndented = true,
       // Add custom converters, naming policies, etc.
   };
   ```
3. Replace line-ending customization:
   ```csharp
   // Before
   SnapshotSettings.CreateStringWriter = () => new StringWriter(CultureInfo.InvariantCulture)
   {
       NewLine = "\n"
   };

   // After
   SnapshotSettings.NewLine = "\n";
   ```
4. Regenerate all `.snap` file snapshots: `UPDATE=true dotnet test`
5. Manually verify and update any inline snapshots (`ShouldMatchInlineSnapshot` string literals)

### Serialization behavior differences (STJ vs Newtonsoft)
- Enums serialize as integers by default (same as Newtonsoft). Use `JsonStringEnumConverter` in `JsonSerializerOptions.Converters` for string names.
- `DateTime`/`DateTimeOffset` use ISO 8601 (same as Newtonsoft, minor formatting differences possible).
- Fields are not serialized by default. Set `JsonSerializerOptions.IncludeFields = true` to include them.
- Inherited properties: STJ serializes base class properties before derived class, Newtonsoft interleaves by declaration order.

## [1.5.0] - 2026-03-05

### Changed
- Target frameworks updated to .NET 10.0 and .NET 8.0 (dropped older TFMs)
- Upgraded to C# 14 with nullable reference types enabled
- Migrated to `.slnx` solution format
- Updated dependencies

### Added
- CI workflow (GitHub Actions)
- OSS repository files (LICENSE, CONTRIBUTING.md, CHANGELOG.md, SECURITY.md, issue/PR templates)
- README with CI badge, install command, and supported frameworks table
- NuGet package now includes README

### Fixed
- Bug fixes in snapshot comparison and update logic

## [1.4.1] - 2023-02-19

### Fixed
- Unified serializers for diff and saving snapshots

## [1.4.0] - 2023-02-17

### Added
- Option to exclude properties from diff via `JsonDiffOptions`

## [1.3.1] - 2020-12-09

### Changed
- Switched to netstandard from netcoreapp3.1

### Added
- Package icon

## [1.2.0] - 2020-06-26

### Added
- Configurable file extensions, paths, and serialization via `SnapshotSettings`
- Support for forcing CRLF or LF line endings on snapshot files

## [1.1.0] - 2020-03-26

### Added
- `JestAssert` static class as alternative to extension methods

## [1.0.0] - 2020-03-09

### Added
- Initial release
- `ShouldMatchSnapshot` extension method with automatic snapshot file management
