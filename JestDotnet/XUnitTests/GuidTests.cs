using System;
using JestDotnet;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests
{
    public class GuidTests
    {
        [Fact]
        public void ShouldMatchGuid()
        {
            var actual = new GuidPerson
            {
                Name = "Bam",
                Id = new Guid("18fd3cb5-5442-4a02-b3fe-27ebaaf138db")
            };

            var expected = new GuidPerson
            {
                Name = "Bam",
                Id = new Guid("18fd3cb5-5442-4a02-b3fe-27ebaaf138db")
            };

            JestAssert.ShouldMatchObject(actual, expected);
        }

        [Fact]
        public void ShouldMatchGuidExtension()
        {
            var actual = new GuidPerson
            {
                Name = "Bam",
                Id = new Guid("18fd3cb5-5442-4a02-b3fe-27ebaaf138db")
            };

            actual.ShouldMatchSnapshot();
        }
    }
}
