# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

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
