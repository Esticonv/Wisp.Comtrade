using System;
using Wisp.Comtrade.Models;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class DateTimeParseTest
{
    [Fact]
    public void ParserTest()
    {
        const string dateTimeStr1 = @"06/04/2012,06:42:59.391376";
        const string dateTimeStr2 = @"6/4/2012,06:42:59.391376";
        const string dateTimeStr3 = @"6/04/2012,06:42:59.3913768";
        const string dateTimeStr4 = @"06/4/2012,06:42:59.391376834";

        Assert.Equal(new DateTime(2012, 4, 6, 6, 42, 59, 391).AddTicks(3760), ConfigurationHandler.ParseDateTime(dateTimeStr1, out var nanosecond));
        Assert.False(nanosecond);

        Assert.Equal(new DateTime(2012, 4, 6, 6, 42, 59, 391).AddTicks(3760), ConfigurationHandler.ParseDateTime(dateTimeStr2, out nanosecond));
        Assert.False(nanosecond);

        Assert.Equal(new DateTime(2012, 4, 6, 6, 42, 59, 391).AddTicks(3768), ConfigurationHandler.ParseDateTime(dateTimeStr3, out nanosecond));
        Assert.True(nanosecond);

        Assert.Equal(new DateTime(2012, 4, 6, 6, 42, 59, 391).AddTicks(3768), ConfigurationHandler.ParseDateTime(dateTimeStr4, out nanosecond));
        Assert.True(nanosecond);
    }
}
