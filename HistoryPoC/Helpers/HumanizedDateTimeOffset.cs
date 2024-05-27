using System;
using System.Globalization;
using Humanizer;

namespace HistoryPoC.Helpers;

public record HumanizedDateTimeOffset
{
    public DateTimeOffset Value { get; }

    public HumanizedDateTimeOffset(DateTimeOffset value)
    {
        Value = value;
    }

    public override string ToString() => Value.Humanize(DateTimeOffset.Now, CultureInfo.InvariantCulture);
}