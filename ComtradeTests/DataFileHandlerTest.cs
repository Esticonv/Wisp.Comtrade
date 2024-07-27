using Xunit;

namespace Wisp.Comtrade.Tests;

public class DataFileHandlerTest
{
    [Fact]
    public void TestByteCount()
    {
        Assert.Equal(12, DataFileHandler.GetByteCountInOneSample(1, 1, DataFileType.Binary));
        Assert.Equal(22, DataFileHandler.GetByteCountInOneSample(5, 17, DataFileType.Binary));
        Assert.Equal(32, DataFileHandler.GetByteCountInOneSample(5, 17, DataFileType.Float32));
        Assert.Equal(32, DataFileHandler.GetByteCountInOneSample(5, 17, DataFileType.Binary32));
    }

    [Fact]
    public void TestDigitalByteCount()
    {
        Assert.Equal(2, DataFileHandler.GetDigitalByteCount(7));
        Assert.Equal(2, DataFileHandler.GetDigitalByteCount(16));
        Assert.Equal(4, DataFileHandler.GetDigitalByteCount(17));
        Assert.Equal(4, DataFileHandler.GetDigitalByteCount(32));
    }
}
