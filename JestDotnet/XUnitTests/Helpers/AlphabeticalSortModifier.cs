using System.Linq;
using System.Text.Json.Serialization.Metadata;

namespace XUnitTests.Helpers;

public static class AlphabeticalSortModifier
{
    public static void SortProperties(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        var sortedProperties = typeInfo.Properties.OrderBy(p => p.Name).ToList();

        for (var i = 0; i < sortedProperties.Count; i++)
        {
            sortedProperties[i].Order = i;
        }
    }
}
