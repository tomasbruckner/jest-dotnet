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
            var testObject = DataGenerator.GenerateComplexObjectData();

            JestAssert.ShouldMatchSnapshot(testObject);
        }
    }
}
