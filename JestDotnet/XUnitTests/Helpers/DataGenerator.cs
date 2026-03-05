using System.Collections.Generic;

namespace XUnitTests.Helpers;

public static class DataGenerator
{
    public static ComplexObject GenerateComplexObjectData()
    {
        return new ComplexObject
        {
            BoolValue = false,
            IntValue = 123,
            StringValue = string.Empty,
            ChildObject = new ChildObject
            {
                IntValue = 33,
                StringValue = "child"
            },
            DateTimeValue = System.DateTime.MaxValue,
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
                            { "\u0420\u0438\u043a\u0440\u043e\u043b\u043b\u0438\u043d\u0433", true },
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
                        IntValue = 312,
                        StringValue1 = "nested2",
                        StringValue2 = "x",
                        IntNullValue = 4,
                        ReadOnlyDictionaryChildren = null
                    }
                }
            }
        };
    }
}
