using System;
using JestDotnet;
using Xunit;
using XUnitTests.Helpers;

namespace XUnitTests;

public class SerializationBehaviorTests
{
    [Fact]
    public void ShouldMatchEnumDefaults()
    {
        var obj = new EnumObject
        {
            Color = Color.Green,
            NullableColor = null,
            Permissions = Permissions.Read | Permissions.Write,
        };

        JestAssert.ShouldMatchSnapshot(obj);
    }

    [Fact]
    public void ShouldMatchEnumAllValues()
    {
        var obj = new EnumObject
        {
            Color = Color.Blue,
            NullableColor = Color.Red,
            Permissions = Permissions.All,
        };

        JestAssert.ShouldMatchSnapshot(obj);
    }

    [Fact]
    public void ShouldMatchDateTime()
    {
        var obj = new DateTimeObject
        {
            DateTime = new DateTime(2024, 6, 15, 14, 30, 45),
            NullableDateTime = null,
            DateTimeOffset = new DateTimeOffset(2024, 6, 15, 14, 30, 45, TimeSpan.FromHours(2)),
            NullableDateTimeOffset = null,
        };

        JestAssert.ShouldMatchSnapshot(obj);
    }

    [Fact]
    public void ShouldMatchDateTimeWithValues()
    {
        var obj = new DateTimeObject
        {
            DateTime = new DateTime(2024, 12, 31, 23, 59, 59, 999),
            NullableDateTime = new DateTime(2020, 1, 1),
            DateTimeOffset = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            NullableDateTimeOffset = new DateTimeOffset(2024, 6, 15, 14, 30, 45, TimeSpan.FromHours(-5)),
        };

        JestAssert.ShouldMatchSnapshot(obj);
    }

    [Fact]
    public void ShouldMatchDateTimeUtc()
    {
        var obj = new DateTimeObject
        {
            DateTime = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc),
            NullableDateTime = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc),
            DateTimeOffset = DateTimeOffset.UnixEpoch,
            NullableDateTimeOffset = DateTimeOffset.MinValue,
        };

        JestAssert.ShouldMatchSnapshot(obj);
    }
}
