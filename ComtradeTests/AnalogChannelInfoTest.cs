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
			const string str=@"  8,F8-VN               ,N,,V     ,     0.012207,1,2,-32767,32767, 330000.0,100.0,S";			
			var channelInfo=new AnalogChannelInformation(str);

			Assert.AreEqual(8,			channelInfo.Index);
			Assert.AreEqual("F8-VN",	channelInfo.name);
			Assert.AreEqual("N",		channelInfo.phase);
			Assert.AreEqual("",			channelInfo.circuitComponent);
			Assert.AreEqual("V",		channelInfo.units);
			Assert.AreEqual(0.012207d,	channelInfo.a, 0.001d);
			Assert.AreEqual(1,			channelInfo.b, 0.001d);
			Assert.AreEqual(2,			channelInfo.skew, 0.001d);
			Assert.AreEqual(-32767,		channelInfo.Min, 0.001d);
			Assert.AreEqual(32767,		channelInfo.Max, 0.001d);
			Assert.AreEqual(330000.0d,	channelInfo.primary, 0.001d);
			Assert.AreEqual(100.0,		channelInfo.secondary, 0.001d);
			Assert.AreEqual(false,		channelInfo.isPrimary);
		}
	}
}
