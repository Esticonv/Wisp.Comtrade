using System.Collections.Generic;
using Wisp.Comtrade.Models;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class DataFileSampleTest
{
    [Fact]
    public void ASCIIReadingTest()
    {
        const string str = "5 ,667 , -760, 1274,72,, 3.4028235e38,-3.4028235e38,0 ,0,0 ,0,1,1";
        var sample = new DataFileSample(str, 6, 6);

        Assert.Equal(5, sample.Number);
        Assert.Equal(667, sample.Timestamp);
        Assert.Equal(6, sample.AnalogValues.Length);
        Assert.Equal(-760, sample.AnalogValues[0]);
        Assert.Equal(1274, sample.AnalogValues[1]);
        Assert.Equal(72, sample.AnalogValues[2]);
        Assert.Equal(0, sample.AnalogValues[3]);
        Assert.Equal(3.4028235e38, sample.AnalogValues[4]);
        Assert.Equal(-3.4028235e38, sample.AnalogValues[5]);
        Assert.Equal(6, sample.DigitalValues.Length);
        Assert.False(sample.DigitalValues[0]);
        Assert.False(sample.DigitalValues[1]);
        Assert.False(sample.DigitalValues[2]);
        Assert.False(sample.DigitalValues[3]);
        Assert.True(sample.DigitalValues[4]);
        Assert.True(sample.DigitalValues[5]);
    }

    [Fact]
    public void CommonBinaryReadingTest()
    {
        byte[] bytes = {
            0x05, 0x00, 0x50, 0x00,
            0x9B, 0x02, 0x00, 0x00,
            0x08, 0xFD,
            0xFA, 0x04,
            0x48, 0x00,
            0x3D, 0x00,
            0x74, 0xFF,
            0x0A, 0xFE,
            0x30, 0x00
        };

        var sample = new DataFileSample(bytes, DataFileType.Binary, 6, 6);

        Assert.Equal(5242885, sample.Number);
        Assert.Equal(667, sample.Timestamp);
        Assert.Equal(6, sample.AnalogValues.Length);
        Assert.Equal(-760, sample.AnalogValues[0]);
        Assert.Equal(1274, sample.AnalogValues[1]);
        Assert.Equal(72, sample.AnalogValues[2]);
        Assert.Equal(61, sample.AnalogValues[3]);
        Assert.Equal(-140, sample.AnalogValues[4]);
        Assert.Equal(-502, sample.AnalogValues[5]);
        Assert.Equal(6, sample.DigitalValues.Length);
        Assert.False(sample.DigitalValues[0]);
        Assert.False(sample.DigitalValues[1]);
        Assert.False(sample.DigitalValues[2]);
        Assert.False(sample.DigitalValues[3]);
        Assert.True(sample.DigitalValues[4]);
        Assert.True(sample.DigitalValues[5]);
    }

    [Fact]
    public void CommonBinaryWritingTest()
    {
        byte[] bytes = {
            0x05, 0x00, 0x50, 0x00,
            0x9B, 0x02, 0x00, 0x00,
            0x08, 0xFD,
            0xFA, 0x04,
            0x48, 0x00,
            0x3D, 0x00,
            0x74, 0xFF,
            0x0A, 0xFE,
            0x30, 0x00
        };

        var sample = new DataFileSample("5242885,667,-760,1274,72,61,-140,-502,0,0,0,0,1,1", 6, 6);

        var analogInformations = new List<AnalogChannel>();
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));
        analogInformations.Add(new AnalogChannel(string.Empty, string.Empty));

        var result = sample.ToByteDAT(DataFileType.Binary, analogInformations);

        for (var i = 0; i < bytes.Length; i++) {
            Assert.Equal(bytes[i], result[i]);
        }
    }

    [Fact]
    public void DigitalOnlyBinaryReadingTest()
    {
        byte[] bytes = {
            0x05, 0x00, 0x00, 0x00,
            0x9B, 0x02, 0x00, 0x00,
            0x0F, 0x0F,
            0x5A, 0x5A,
            0x01, 0x00
        };

        var sample = new DataFileSample(bytes, DataFileType.Binary, 0, 33);

        Assert.Equal(5, sample.Number);
        Assert.Equal(667, sample.Timestamp);
        Assert.Empty(sample.AnalogValues);
        Assert.Equal(33, sample.DigitalValues.Length);

        Assert.True(sample.DigitalValues[0]);
        Assert.True(sample.DigitalValues[1]);
        Assert.True(sample.DigitalValues[2]);
        Assert.True(sample.DigitalValues[3]);
        Assert.False(sample.DigitalValues[4]);
        Assert.False(sample.DigitalValues[5]);
        Assert.False(sample.DigitalValues[6]);
        Assert.False(sample.DigitalValues[7]);

        Assert.True(sample.DigitalValues[8]);
        Assert.True(sample.DigitalValues[9]);
        Assert.True(sample.DigitalValues[10]);
        Assert.True(sample.DigitalValues[11]);
        Assert.False(sample.DigitalValues[12]);
        Assert.False(sample.DigitalValues[13]);
        Assert.False(sample.DigitalValues[14]);
        Assert.False(sample.DigitalValues[15]);

        Assert.False(sample.DigitalValues[16]);
        Assert.True(sample.DigitalValues[17]);
        Assert.False(sample.DigitalValues[18]);
        Assert.True(sample.DigitalValues[19]);
        Assert.True(sample.DigitalValues[20]);
        Assert.False(sample.DigitalValues[21]);
        Assert.True(sample.DigitalValues[22]);
        Assert.False(sample.DigitalValues[23]);

        Assert.False(sample.DigitalValues[24]);
        Assert.True(sample.DigitalValues[25]);
        Assert.False(sample.DigitalValues[26]);
        Assert.True(sample.DigitalValues[27]);
        Assert.True(sample.DigitalValues[28]);
        Assert.False(sample.DigitalValues[29]);
        Assert.True(sample.DigitalValues[30]);
        Assert.False(sample.DigitalValues[31]);

        Assert.True(sample.DigitalValues[32]);
    }
}
