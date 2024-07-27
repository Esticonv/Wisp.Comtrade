using System;
using System.Reflection;
using Xunit;

namespace Wisp.Comtrade.Tests;

public class RecordReaderTest
{
    private readonly string _pathToPojectRoot;

    public RecordReaderTest()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var projectRoot = System.IO.Directory.GetParent(assemblyLocation)!.Parent!.Parent!.Parent!;
        _pathToPojectRoot = projectRoot.FullName;
    }

    [Fact]
    public void TestNotSupportedExtensions()
    {
        Assert.Throws<InvalidOperationException>(() => new RecordReader("notComtradeExtensions.trr"));
    }

    [Theory]
    [InlineData("\\ExampleSet1\\", "sample_ascii.dat")]
    [InlineData("\\ExampleSet1\\", "sample_bin.DAT")]
    [InlineData("\\ExampleSet1\\", "sample_ascii.cFg")]
    [InlineData("\\ExampleSet1\\", "sample_bin.cfg")]

    [InlineData("\\ExampleSet2\\", "1.dat")]
    [InlineData("\\ExampleSet2\\", "2.DAT")]
    [InlineData("\\ExampleSet2\\", "3.cFg")]
    [InlineData("\\ExampleSet2\\", "4.cfg")]
    [InlineData("\\ExampleSet2\\", "5.cfg")]
    public void TestOpenFile(string path, string fileName)
    {
        var record = new RecordReader(_pathToPojectRoot + path + fileName);
        record.GetTimeLine();
        record.GetAnalogPrimaryChannel(0);
        record.GetDigitalChannel(0);
    }
}
