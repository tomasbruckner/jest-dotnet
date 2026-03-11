using System;
using System.Linq;
using System.Text.Json.Serialization.Metadata;

namespace JestDotnet.Core.Settings;

/// <summary>
///     Sorts JSON properties alphabetically by name
/// </summary>
public static class AlphabeticalSortModifier
{
    /// <summary>
    ///     Modifier that sorts properties alphabetically. Use with DefaultJsonTypeInfoResolver.Modifiers.
    /// </summary>
    public static void SortProperties(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        var sortedProperties = typeInfo.Properties.OrderBy(p => p.Name, StringComparer.Ordinal).ToList();

        for (var i = 0; i < sortedProperties.Count; i++)
        {
            sortedProperties[i].Order = i;
        }
    }
}
