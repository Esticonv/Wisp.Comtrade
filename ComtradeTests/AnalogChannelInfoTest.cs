using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wisp.Comtrade
{
	[TestClass]
	public class AnalogChannelInfoTest
	{
		[TestMethod]
		public void ParserTest()
		{
			const string str = @"  8,F8-VN               ,N,,V     ,     0.012207,1,2,-32767,32767, 330000.0,100.0,S";
			var channelInfo = new AnalogChannelInformation(str);

			Assert.AreEqual(8, channelInfo.Index);
			Assert.AreEqual("F8-VN", channelInfo.Name);
			Assert.AreEqual("N", channelInfo.Phase);
			Assert.AreEqual("", channelInfo.CircuitComponent);
			Assert.AreEqual("V", channelInfo.Units);
			Assert.AreEqual(0.012207d, channelInfo.MultiplierA, 0.001d);
			Assert.AreEqual(1, channelInfo.MultiplierB, 0.001d);
			Assert.AreEqual(2, channelInfo.Skew, 0.001d);
			Assert.AreEqual(-32767, channelInfo.Min, 0.001d);
			Assert.AreEqual(32767, channelInfo.Max, 0.001d);
			Assert.AreEqual(330000.0d, channelInfo.Primary, 0.001d);
			Assert.AreEqual(100.0, channelInfo.Secondary, 0.001d);
			Assert.AreEqual(false, channelInfo.IsPrimary);
		}		
	}
}
