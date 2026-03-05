using System;
using JestDotnet.Core.Exceptions;

namespace JestDotnet.Core;

internal static class SnapshotUpdater
{
    private const string UpdateSnapshotEnvironmentVariable = "UPDATE";
    private const string ContinuousIntegrationEnvironmentVariable = "CI";

    internal static void TryUpdateSnapshot<T>(string path, T actual, string? message) where T : notnull
    {
        if (!ShouldUpdate())
        {
            throw new SnapshotMismatch(message ?? "Snapshot mismatch");
        }

        SnapshotResolver.StoreSnapshotData(path, actual);
    }

    internal static void TryUpdateMissingSnapshot<T>(string path, T actual) where T : notnull
    {
        if (!ShouldUpdateIfEmpty())
        {
            throw new SnapshotDoesNotExist("Snapshot does not exist");
        }

        SnapshotResolver.StoreSnapshotData(path, actual);
    }

    private static bool ShouldUpdateIfEmpty()
    {
        return Environment.GetEnvironmentVariable(ContinuousIntegrationEnvironmentVariable) != "true";
    }

    private static bool ShouldUpdate()
    {
        return Environment.GetEnvironmentVariable(UpdateSnapshotEnvironmentVariable) == "true";
    }
}
