using System;
using System.Collections.Generic;
using JestDotnet;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests
{
    public class ComplexObjectTests
    {
        [Fact]
        public void ShouldMatchDynamicObject()
        {
            var testObject = new ComplexObject
            {
                BoolValue = false,
                IntValue = 123,
                StringValue = string.Empty,
                ChildObject = new ChildObject
                {
                    IntValue = 33,
                    StringValue = "child"
                },
                DateTimeValue = DateTime.MaxValue,
                IntNullValue = null,
                Children = new Dictionary<string, DictionaryChildObject>
                {
                    {
                        "first", new DictionaryChildObject
                        {
                            IntValue = 312,
                            StringValue1 = "nested1",
                            StringValue2 = null,
                            IntNullValue = null,
                            ReadOnlyDictionaryChildren = new Dictionary<string, bool>
                            {
                                {"key1", false},
                                {"key2", true},
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key1",
                                    true
                                },
                                {"Рикроллинг", true},
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key3",
                                    true
                                },
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key4",
                                    true
                                },
                                {"4", true}
                            }
                        }
                    },
                    {
                        "second", new DictionaryChildObject
                        {
                            IntValue = 312,
                            StringValue1 = "nested2",
                            StringValue2 = "x",
                            IntNullValue = 4,
                            ReadOnlyDictionaryChildren = new Dictionary<string, bool>()
                        }
                    },
                    {
                        "third", new DictionaryChildObject
                        {
                            IntValue = 312,
                            StringValue1 = "nested2",
                            StringValue2 = "x",
                            IntNullValue = 4,
                            ReadOnlyDictionaryChildren = null
                        }
                    }
                }
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }
    }
}
