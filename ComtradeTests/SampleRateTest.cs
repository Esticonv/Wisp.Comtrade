using Xunit;

namespace Wisp.Comtrade.Tests;

public class SampleRateTest
{
    [Fact]
    public void ParseTest()
    {
        const string str = @" 0 ,  1360";
        var sampleRate = new SampleRate(str);

        Assert.Equal(0, sampleRate.SamplingFrequency, 0.1);
        Assert.Equal(1360, sampleRate.LastSampleNumber);
    }
}
