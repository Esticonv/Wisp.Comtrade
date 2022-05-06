using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Wisp.Comtrade
{
	[TestClass]
	public class DigitalChannelInfoTest
	{
		[TestMethod]
		public void ParserTest()
		{
			const string str=@"  4,W8a_KQC C    Off    ,,,0";			
			var channelInfo=new DigitalChannelInformation(str);

			Assert.AreEqual(4,					channelInfo.Index);
			Assert.AreEqual("W8a_KQC C    Off", channelInfo.name);
			Assert.AreEqual("",					channelInfo.phase);
			Assert.AreEqual("",					channelInfo.circuitComponent);
			Assert.AreEqual(false,				channelInfo.normalState);	
		}
	}
}

