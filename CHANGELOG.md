# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

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
