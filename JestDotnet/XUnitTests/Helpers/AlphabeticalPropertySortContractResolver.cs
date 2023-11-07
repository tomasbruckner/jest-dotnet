using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XUnitTests.Helpers
{
    public sealed class AlphabeticalPropertySortContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            // Sort the properties alphabetically by property name
            return properties.OrderBy(p => p.PropertyName).ToList();
        }
    }
}
