using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JestDotnet.Core.Settings
{
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
        ///     default JSON serializer creator
        /// </summary>
        public static readonly Func<JsonSerializer> DefaultCreateJsonSerializer = JsonSerializer.CreateDefault;

        /// <summary>
        ///     JSON serializer creator
        /// </summary>
        public static Func<JsonSerializer> CreateJsonSerializer = DefaultCreateJsonSerializer;

        /// <summary>
        ///     default JToken writer creator
        /// </summary>
        public static readonly Func<JTokenWriter> DefaultCreateJTokenWriter = () => new JTokenWriter();

        /// <summary>
        ///     JToken writer creator
        /// </summary>
        public static Func<JTokenWriter> CreateJTokenWriter = DefaultCreateJTokenWriter;

        /// <summary>
        ///     default string writer creator
        /// </summary>
        public static readonly Func<StringWriter> DefaultCreateStringWriter =
            () => new StringWriter(CultureInfo.InvariantCulture);

        /// <summary>
        ///     string writer creator
        /// </summary>
        public static Func<StringWriter> CreateStringWriter = DefaultCreateStringWriter;

        /// <summary>
        ///     default text writer creator
        /// </summary>
        public static readonly Func<StringWriter, JsonTextWriter> DefaultCreateTextWriter = stringWriter =>
            new JsonTextWriter(stringWriter) {Formatting = Formatting.Indented};

        /// <summary>
        ///     text writer creator
        /// </summary>
        public static Func<StringWriter, JsonTextWriter> CreateTextWriter = DefaultCreateTextWriter;
    }
}
