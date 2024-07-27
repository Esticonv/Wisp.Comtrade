using System;
using Wisp.Comtrade.Models;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class RecordWriterTest
{
    private RecordWriter GetWriterToTest()
    {
        var writer = new RecordWriter();
        writer.AddAnalogChannel(new AnalogChannel("channel1a", "A"));
        writer.AddAnalogChannel(new AnalogChannel("channel2a", "B"));
        writer.AddAnalogChannel(new AnalogChannel("channel3a", "C"));
        writer.AddDigitalChannel(new DigitalChannel("channel1b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel2b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel3b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel4b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel5b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel6b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel7b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel8b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel9b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel10b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel11b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel12b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel13b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel14b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel15b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel16b", ""));
        writer.AddDigitalChannel(new DigitalChannel("channel17b", ""));

        writer.AddSample(0,
                         [0, 0, 0],
                         [
                             true, true, true, true,
                             true, true, true, true,
                             true, true, true, true,
                             true, true, true, true,
                             true
                         ]);

        writer.AddSample(500,
                         [1.0, 2.0, 3.0],
                         [
                             false, false, false, false,
                             false, false, false, false,
                             false, false, false, false,
                             false, false, false, false,
                             false
                         ]);

        writer.AddSample(1000,
                         [-1.0, 2.0, -3.5],
                         [
                             false, false, false, false,
                             true, true, true, true,
                             false, true, false, true,
                             true, false, true, false,
                             true
                         ]);

        writer.AddSample(1500,
                         [5.0, 5.0, 5.0],
                         [
                             false, false, false, false,
                             true, true, true, true,
                             false, true, false, true,
                             true, false, true, false,
                             true
                         ]);

        writer.StartTime = new DateTime(1234567890);
        writer.TriggerTime = new DateTime(1234569000);
        return writer;
    }

    private void ReaderAsserts(string fullPath)
    {
        var reader = new RecordReader(fullPath);
        Assert.Equal(3, reader.Configuration.AnalogChannelInformationList.Count);
        Assert.Equal(17, reader.Configuration.DigitalChannelInformationList.Count);

        var timeLine = reader.GetTimeLine();
        var analogs1 = reader.GetAnalogPrimaryChannel(0);
        var analogs2 = reader.GetAnalogPrimaryChannel(1);
        var analogs3 = reader.GetAnalogPrimaryChannel(2);
        var digitals1 = reader.GetDigitalChannel(0);
        var digitals5 = reader.GetDigitalChannel(4);
        var digitals17 = reader.GetDigitalChannel(16);


        Assert.Equal(0, timeLine[0], 0.01);
        Assert.Equal(500, timeLine[1], 0.01);
        Assert.Equal(1000, timeLine[2], 0.01);
        Assert.Equal(0, analogs1[0], 0.01);
        Assert.Equal(1, analogs1[1], 0.01);
        Assert.Equal(-1, analogs1[2], 0.01);
        Assert.Equal(0, analogs2[0], 0.01);
        Assert.Equal(2, analogs2[1], 0.01);
        Assert.Equal(2, analogs2[2], 0.01);
        Assert.Equal(0, analogs3[0], 0.01);
        Assert.Equal(3, analogs3[1], 0.01);
        Assert.Equal(-3.5, analogs3[2], 0.01);

        Assert.True(digitals1[0]);
        Assert.False(digitals1[1]);
        Assert.False(digitals1[2]);
        Assert.True(digitals5[0]);
        Assert.False(digitals5[1]);
        Assert.True(digitals5[2]);
        Assert.True(digitals17[0]);
        Assert.False(digitals17[1]);
        Assert.True(digitals17[2]);

        Assert.Equal(new DateTime(1234567890), reader.Configuration.StartTime);
        Assert.Equal(new DateTime(1234569000), reader.Configuration.TriggerTime);
    }

    [Fact]
    public void SaveToFileTwoFilesAsciiTest()
    {
        var fullPath = "asciiMulti.cfg";
        var writer = GetWriterToTest();
        writer.SaveToFile(fullPath, false, DataFileType.ASCII);
        ReaderAsserts(fullPath);
    }

    [Fact]
    public void SaveToFileTwoFilesBinaryTest()
    {
        var fullPath = "binaryMulti.cfg";
        var writer = GetWriterToTest();
        writer.SaveToFile(fullPath, false);
        ReaderAsserts(fullPath);
    }

    [Fact]
    public void SaveToFileSingleAsciiTest()
    {
        var fullPath = "asciiSingle.cff";
        var writer = GetWriterToTest();
        writer.SaveToFile(fullPath, true, DataFileType.ASCII);
        ReaderAsserts(fullPath);
    }

    [Fact]
    public void SaveToFileSingleFileBinaryTest()
    {
        var fullPath = "binarySingle.cff";
        var writer = GetWriterToTest();
        writer.SaveToFile(fullPath, true);
        ReaderAsserts(fullPath);
    }
}
