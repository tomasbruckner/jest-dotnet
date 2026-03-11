using System;
using System.Collections.Generic;
using JestDotnet;
using JestDotnet.Core.Settings;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests;

public class SettingsTests
{
    [Fact]
    public void ShouldMatchCustomSnapExtension()
    {
        SnapshotSettings.SnapshotExtension = "snap2";

        try
        {
            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }
        finally
        {
            SnapshotSettings.SnapshotExtension = SnapshotSettings.DefaultSnapshotExtension;
        }
    }

    [Fact]
    public void ShouldMatchLinuxLineEnding()
    {
        SnapshotSettings.NewLine = "\n";

        try
        {
            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }
        finally
        {
            SnapshotSettings.NewLine = SnapshotSettings.DefaultNewLine;
        }
    }

    [Fact]
    public void ShouldMatchWindowsLineEnding()
    {
        SnapshotSettings.NewLine = "\r\n";

        try
        {
            var testObject = new Person
            {
                Age = 13,
                DateOfBirth = new DateTime(2008, 7, 7),
                FirstName = "John",
                LastName = "Bam"
            };

            JestAssert.ShouldMatchSnapshot(testObject);
        }
        finally
        {
            SnapshotSettings.NewLine = SnapshotSettings.DefaultNewLine;
        }
    }

    [Fact]
    public void ShouldSortDictionaryAlphabetically()
    {
        var testObject = new SortedDictionary<string, object>
        {
            ["Zebra"] = 1,
            ["Apple"] = 2,
            ["Mango"] = new SortedDictionary<string, object>
            {
                ["Zulu"] = "z",
                ["Alpha"] = "a"
            }
        };

        JestAssert.ShouldMatchSnapshot(testObject);
    }
}
