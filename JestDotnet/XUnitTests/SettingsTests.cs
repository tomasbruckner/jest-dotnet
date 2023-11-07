using System;
using System.Globalization;
using System.IO;
using JestDotnet;
using JestDotnet.Core.Settings;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests
{
    public class SettingsTests
    {
        [Fact]
        public void ShouldMatchCustomSnapExtension()
        {
            SnapshotSettings.CreateStringWriter = () => new StringWriter(CultureInfo.InvariantCulture)
            {
                NewLine = "\n"
            };

            SnapshotSettings.SnapshotExtension = "snap2";

            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);

            SnapshotSettings.SnapshotExtension = SnapshotSettings.DefaultSnapshotExtension;
        }

        [Fact]
        public void ShouldMatchLinuxLineEnding()
        {
            SnapshotSettings.CreateStringWriter = () => new StringWriter(CultureInfo.InvariantCulture)
            {
                NewLine = "\n"
            };

            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }

        [Fact]
        public void ShouldMatchWindowsLineEnding()
        {
            SnapshotSettings.CreateStringWriter = () => new StringWriter(CultureInfo.InvariantCulture)
            {
                NewLine = "\r\n"
            };

            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }

        [Fact]
        public void ShouldSortAlphabeticallySimple()
        {
            SnapshotSettings.CreateJsonSerializer = () =>
            {
                var serializer = SnapshotSettings.DefaultCreateJsonSerializer();
                serializer.ContractResolver = new AlphabeticalPropertySortContractResolver();

                return serializer;
            };

            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }

        [Fact]
        public void ShouldSortAlphabeticallyComplex()
        {
            SnapshotSettings.CreateJsonSerializer = () =>
            {
                var serializer = SnapshotSettings.DefaultCreateJsonSerializer();
                serializer.ContractResolver = new AlphabeticalPropertySortContractResolver();

                return serializer;
            };

            var testObject = DataGenerator.GenerateComplexObjectData();

            JestAssert.ShouldMatchSnapshot(testObject);
        }
    }
}
