using System;
using System.Collections.Generic;
using System.Text.Json.JsonDiffPatch;
using JestDotnet;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests
{
    public class ExcludePathsTests
    {
        [Fact]
        public void ShouldIgnoreIntValue()
        {
            const int invalidValue = -1;

            var testObject = new ComplexObject
            {
                BoolValue = false,
                IntValue = invalidValue,
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
                                { "key1", false },
                                { "key2", true },
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key1",
                                    true
                                },
                                { "Рикроллинг", true },
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key3",
                                    true
                                },
                                {
                                    "very.very.very.very.very.very.very.very.very.very.very.very.very.long.key4",
                                    true
                                },
                                { "4", true }
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
                            IntValue = invalidValue,
                            StringValue1 = "nested2",
                            StringValue2 = "x",
                            IntNullValue = 4,
                            ReadOnlyDictionaryChildren = null
                        }
                    }
                }
            };

            JestAssert.ShouldMatchSnapshot(
                testObject,
                "",
                new JsonDiffOptions
                {
                    PropertyFilter = (s, _) => s != "IntValue",
                }
            );
        }

        [Fact]
        public void ShouldIgnoreSimpleIntValue()
        {
            const int invalidValue = -1;
            const int validValue = 2;

            var testObject = new Dictionary<string, Dictionary<string, int>>
            {
                {
                    "a", new Dictionary<string, int>
                    {
                        { "exclude", invalidValue },
                        { "notExclude", 15}
                    }
                },
                {
                    "exclude", new Dictionary<string, int>
                    {
                        { "c", invalidValue }
                    }
                }
            };

            JestAssert.ShouldMatchSnapshot(
                testObject,
                "",
                new JsonDiffOptions
                {
                    JsonElementComparison = JsonElementComparison.Semantic,
                    PropertyFilter = (s, _) => s != "exclude",
                }
            );
        }

        [Fact]
        public void ShouldIgnoreSimpleIntValue_Object()
        {
            var actual = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            var expected = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = ""
            };

            JestAssert.ShouldMatchObject(actual,expected, new JsonDiffOptions
            {
                PropertyFilter = (s, context) => s != "LastName"
            });
        }
    }
}
