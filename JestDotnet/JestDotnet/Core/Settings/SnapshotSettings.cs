using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace JestDotnet.Core.Settings;

public static class SnapshotSettings
{
    /// <summary>
    ///     Default snapshot extension
    /// </summary>
    public const string DefaultSnapshotExtension = "snap";

    /// <summary>
    ///     default snapshot directory
    /// </summary>
    public const string DefaultSnapshotDirectory = "__snapshots__";

    /// <summary>
    ///     snapshot extension
    /// </summary>
    public static string SnapshotExtension = DefaultSnapshotExtension;

    /// <summary>
    ///     snapshot directory
    /// </summary>
    public static string SnapshotDirectory = DefaultSnapshotDirectory;

    /// <summary>
    ///     default snapshot dot extension creator
    /// </summary>
    public static readonly Func<string> DefaultCreateSnapshotDotExtension = () => $".{SnapshotExtension}";

    /// <summary>
    ///     snapshot dot extension creator
    /// </summary>
    public static Func<string> CreateSnapshotDotExtension = DefaultCreateSnapshotDotExtension;

    /// <summary>
    ///     default function that creates snapshot path
    /// </summary>
    public static readonly Func<(string sourceFilePath, string memberName, string hint), string> DefaultCreatePath =
        SnapshotResolver.CreatePath;

    /// <summary>
    ///     function that creates snapshot path
    /// </summary>
    public static Func<(string sourceFilePath, string memberName, string hint), string> CreatePath =
        DefaultCreatePath;

    /// <summary>
    ///     default JSON serializer options creator
    /// </summary>
    public static readonly Func<JsonSerializerOptions> DefaultCreateSerializerOptions = () =>
        new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { AlphabeticalSortModifier.SortProperties }
            },
            Converters = { new RuntimePolymorphicConverterFactory() },
        };

    /// <summary>
    ///     JSON serializer options creator
    /// </summary>
    public static Func<JsonSerializerOptions> CreateSerializerOptions = DefaultCreateSerializerOptions;

    /// <summary>
    ///     default newline string
    /// </summary>
    public const string DefaultNewLine = "\n";

    /// <summary>
    ///     newline string used in JSON output
    /// </summary>
    public static string NewLine = DefaultNewLine;

    /// <summary>
    ///     default diff options creator
    /// </summary>
    public static readonly Func<JsonDiffOptions?> DefaultCreateDiffOptions = () => null;

    /// <summary>
    ///     diff options creator
    /// </summary>
    public static Func<JsonDiffOptions?> CreateDiffOptions = DefaultCreateDiffOptions;

    private static readonly Dictionary<Type, Func<object, string>> PreSerializers = new();

    public static void AddPreSerializer<T>(Func<T, string> serializer)
    {
        PreSerializers[typeof(T)] = obj => serializer((T)obj);
    }

    public static void ClearPreSerializers()
    {
        PreSerializers.Clear();
    }

    internal static bool TryGetPreSerializer(Type type, out Func<object, string>? serializer)
    {
        return PreSerializers.TryGetValue(type, out serializer);
    }
}
