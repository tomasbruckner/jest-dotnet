using System;

namespace XUnitTests.Helpers;

public enum Color
{
    Red,
    Green,
    Blue,
}

[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    All = Read | Write | Execute,
}

public class EnumObject
{
    public Color Color { get; set; }
    public Color? NullableColor { get; set; }
    public Permissions Permissions { get; set; }
}

public class DateTimeObject
{
    public DateTime DateTime { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
    public DateTimeOffset? NullableDateTimeOffset { get; set; }
}
