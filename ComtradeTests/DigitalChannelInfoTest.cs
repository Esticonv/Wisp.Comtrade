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
			Assert.AreEqual("W8a_KQC C    Off", channelInfo.Name);
			Assert.AreEqual("",					channelInfo.Phase);
			Assert.AreEqual("",					channelInfo.CircuitComponent);
			Assert.AreEqual(false,				channelInfo.NormalState);	
		}
	}
}

