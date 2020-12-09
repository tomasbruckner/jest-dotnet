using System;
using System.Collections.Generic;

namespace XUnitTests.Helpers
{
    public class ComplexObject
    {
        public IDictionary<string, DictionaryChildObject> Children { get; set; }
        public ChildObject ChildObject { get; set; }
        public bool BoolValue { get; set; }
        public int IntValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public string StringValue { get; set; }
        public int? IntNullValue { get; set; }
    }

    public class DictionaryChildObject
    {
        public IReadOnlyDictionary<string, bool> ReadOnlyDictionaryChildren { get; set; }
        public int IntValue { get; set; }
        public string StringValue1 { get; set; }
        public string StringValue2 { get; set; }
        public int? IntNullValue { get; set; }
    }

    public class ChildObject
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }
}
