#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$SCRIPT_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
    echo "Error: .env file not found. Create one with NUGET_API_KEY=your-key" >&2
    exit 1
fi

source "$ENV_FILE"

if [ -z "${NUGET_API_KEY:-}" ] || [ "$NUGET_API_KEY" = "your-api-key-here" ]; then
    echo "Error: Set NUGET_API_KEY in .env" >&2
    exit 1
fi

NUPKG_DIR="$SCRIPT_DIR/nupkg"
rm -rf "$NUPKG_DIR"

echo "Building and packing..."
dotnet pack "$SCRIPT_DIR/JestDotnet/JestDotnet/JestDotnet.csproj" -c Release -o "$NUPKG_DIR"

PACKAGE=$(find "$NUPKG_DIR" -name "*.nupkg" ! -name "*.symbols.nupkg" | head -1)

if [ -z "$PACKAGE" ]; then
    echo "Error: No .nupkg file found" >&2
    exit 1
fi

echo "Pushing $PACKAGE..."
dotnet nuget push "$PACKAGE" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json

echo "Done!"
