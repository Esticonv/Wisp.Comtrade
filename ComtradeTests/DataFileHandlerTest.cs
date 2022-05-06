using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wisp.Comtrade
{
	[TestClass]
	public class DataFileHandlerTest
	{
		[TestMethod]
		public void TestDigitalByteCount()
		{
			Assert.AreEqual(2, DataFileHandler.GetDigitalByteCount(7));
			Assert.AreEqual(2, DataFileHandler.GetDigitalByteCount(16));
			Assert.AreEqual(4, DataFileHandler.GetDigitalByteCount(17));
			Assert.AreEqual(4, DataFileHandler.GetDigitalByteCount(32));		
		}
		
		[TestMethod]
		public void TestByteCount()
		{
			Assert.AreEqual(12, DataFileHandler.GetByteCount(1, 1, DataFileType.Binary));
			Assert.AreEqual(22, DataFileHandler.GetByteCount(5, 17, DataFileType.Binary));
			Assert.AreEqual(32, DataFileHandler.GetByteCount(5, 17, DataFileType.Float32));
			Assert.AreEqual(32, DataFileHandler.GetByteCount(5, 17, DataFileType.Binary32));		
		}
	}
}

