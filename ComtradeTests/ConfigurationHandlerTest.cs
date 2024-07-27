using System;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class ConfigurationHandlerTest
{
    [Fact]
    public void ParserTest()
    {
        const string str = @"MASHUK-W2D-C60-1    ,520                 ,1999
 5,2A, 3D
  1,F1-IA               ,A,,A     ,     0.001953,0,0,-32767,32767,   1000.0,  1.0,S
  2,F2-IB               ,B,,A     ,     0.001953,0,0,-32767,32767,   1000.0,  1.0,S
    1,OSC TRIG     On     ,,,0
  2,W7a_KQC A    Off    ,,,0
  3,W7c_KQC B    Off    ,,,0
50
0
0,  1360
06/04/2012,06:42:59.391076
06/04/2012,06:42:59.895690
BINARY
1.00
";

        var strings = str.Split(GlobalSettings.NewLines, StringSplitOptions.RemoveEmptyEntries);
        var configHandler = new ConfigurationHandler(strings);

        Assert.Equal("MASHUK-W2D-C60-1", configHandler.StationName);
        Assert.Equal("520", configHandler.DeviceId);
        Assert.Equal(ComtradeVersion.V1999, configHandler.Version);
        Assert.Equal(2, configHandler.AnalogChannelsCount);
        Assert.Equal(2, configHandler.AnalogChannelInformationList.Count);
        Assert.Equal(3, configHandler.DigitalChannelsCount);
        Assert.Equal(3, configHandler.DigitalChannelInformationList.Count);
        Assert.Equal(50, configHandler.Frequency, 0.1);
        Assert.Equal(0, configHandler.SamplingRateCount);
        Assert.Single(configHandler.SampleRates);
        //time1
        //time2
        Assert.Equal(DataFileType.Binary, configHandler.DataFileType);
        //add the rest
    }
}
