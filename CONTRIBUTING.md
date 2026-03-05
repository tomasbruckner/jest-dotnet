# Contributing to JestDotnet

Thanks for your interest in contributing!

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (for full multi-target build)

### Build & Test

All commands run from the `JestDotnet/` directory:

```bash
dotnet build    # Build all projects
dotnet test     # Run all tests
```

To update snapshots after intentional changes:

```bash
UPDATE=true dotnet test
```

## Making Changes

1. Fork the repository
2. Create a feature branch from `master` (`git checkout -b my-feature`)
3. Make your changes
4. Ensure all tests pass (`dotnet test` from `JestDotnet/`)
5. Commit your changes
6. Push to your fork and open a Pull Request against `master`

## Guidelines

- Do not commit directly to `master`
- Keep PRs focused — one feature or fix per PR
- Add tests for new functionality
- `TreatWarningsAsErrors` is enabled — fix all warnings before submitting

## Reporting Issues

Use [GitHub Issues](https://github.com/tomasbruckner/jest-dotnet/issues) to report bugs or request features.
