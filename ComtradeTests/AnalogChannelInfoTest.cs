using Wisp.Comtrade.Models;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class AnalogChannelInfoTest
{
    [Fact]
    public void ParserTest()
    {
        const string str = @"  8,F8-VN               ,N,,V     ,     0.012207,1,2,-32767,32767, 330000.0,100.0,S";
        var channelInfo = new AnalogChannel(str);

        Assert.Equal(8, channelInfo.Index);
        Assert.Equal("F8-VN", channelInfo.Name);
        Assert.Equal("N", channelInfo.Phase);
        Assert.Equal("", channelInfo.CircuitComponent);
        Assert.Equal("V", channelInfo.Units);
        Assert.Equal(0.012207, channelInfo.MultiplierA);
        Assert.Equal(1, channelInfo.MultiplierB, 0.001);
        Assert.Equal(2, channelInfo.Skew, 0.001);
        Assert.Equal(-32767, channelInfo.Min, 0.001);
        Assert.Equal(32767, channelInfo.Max, 0.001);
        Assert.Equal(330000, channelInfo.Primary, 0.001);
        Assert.Equal(100, channelInfo.Secondary, 0.001);
        Assert.False(channelInfo.IsPrimary);
    }
}
