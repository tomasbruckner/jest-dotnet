# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

JestDotnet is a snapshot testing library for .NET, inspired by Jest. It serializes objects to JSON, saves snapshots to files, and compares against them on subsequent runs. Published on NuGet as `JestDotnet` (v2.5.0).

## Git Workflow

Never commit directly to `main`. Always create a feature branch for changes.

## Build & Test Commands

All commands run from `JestDotnet/` (the solution directory):

```bash
dotnet build                                          # Build all projects
dotnet test                                           # Run all tests
dotnet test --filter "SimpleTests.ShouldMatchSnapshot" # Run a single test
UPDATE=true dotnet test                               # Update snapshots
```

## Architecture

**Public API** — two equivalent entry points:
- `JestAssert` — static methods: `ShouldMatchSnapshot`, `ShouldMatchInlineSnapshot`, `ShouldMatchObject`
- `JestDotnetExtensions` — same methods as extension methods on `object`

**Core pipeline** (`JestDotnet/Core/`):
- `Serializer` — converts objects to JSON via System.Text.Json (properties sorted alphabetically by default, literal UTF-8 output, HTML chars unescaped via `UnsafeRelaxedJsonEscaping`, double quotes escaped as `\"` not `\u0022`)
- `SnapshotResolver` — reads/writes `.snap` files in `__snapshots__/` directories, derives path from test class filename + method name + optional hint
- `SnapshotComparer` — diffs expected vs actual using `SystemTextJson.JsonDiffPatch`
- `SnapshotUpdater` — orchestrates create/update/fail logic; checks `UPDATE` env var to update snapshots, `CI` env var to prevent snapshot creation in CI
- `SnapshotSettings` — global static configuration (snapshot directory name, file extension, JSON serializer factory, diff options, pre-serializers for custom types)

**Snapshot behavior:**
1. First run: creates snapshot file
2. Subsequent runs: compares serialized object against saved snapshot
3. Mismatch + `UPDATE=true`: overwrites snapshot
4. Mismatch in CI (`CI=true`): throws `SnapshotMismatch`
5. Missing snapshot in CI: throws `SnapshotDoesNotExist`

## PR Checklist

Before creating a PR, include these in the feature branch:

1. **Version bump** — update `<Version>` in `JestDotnet/JestDotnet/JestDotnet.csproj`
2. **CHANGELOG.md** — add entry under new version heading following Keep a Changelog format
3. **README.md** — update if user-facing behavior changed
4. **CLAUDE.md** — update if version, architecture, or configuration changed

## Key Configuration

- Target framework: `net10.0`
- Language version: C# 14
- Nullable reference types: enabled
- `TreatWarningsAsErrors` is enabled (via `Directory.Build.props`)
- Solution format: `.slnx`
- Test framework: xUnit 2.9.3
- Dependencies: System.Text.Json, SystemTextJson.JsonDiffPatch 2.0.0
