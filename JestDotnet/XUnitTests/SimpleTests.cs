using System;
using Xunit;
using XUnitTests.Helpers;
using JestDotnet;
using JestDotnet.Core.Exceptions;

namespace XUnitTests
{
    public class SimpleTests
    {
        [Fact]
        public void ShouldMatchSnapshot()
        {
            var person = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            person.ShouldMatchSnapshot();
        }

        [Fact]
        public void ShouldMatchSnapshotRecursion()
        {
            var person = new PersonRecursion
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam",
                Parent = new PersonRecursion
                {
                    Age = 43,
                    DateOfBirth = new DateTime(1978, 7, 7),
                    FirstName = "James",
                    LastName = "Bam"
                }
            };

            person.ShouldMatchSnapshot();
        }

        [Fact]
        public void ShouldMatchSnapshotMismatch()
        {
            var person = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            Assert.Throws<SnapshotMismatch>(
                () => { person.ShouldMatchSnapshot(); }
            );
        }

        [Fact]
        public void ShouldMatchInlineSnapshot()
        {
            var person = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            person.ShouldMatchInlineSnapshot(
                @"
{
    ""FirstName"": ""John"",
    ""LastName"": ""Bam"",
    ""DateOfBirth"": ""2008-07-07T00:00:00"",
    ""Age"": 13,    
}"
            );
        }

        [Fact]
        public void ShouldMatchObject()
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
                LastName = "Bam"
            };

            actual.ShouldMatchObject(expected);
        }
    }
}
