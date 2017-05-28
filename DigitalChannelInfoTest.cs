/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 23.05.2017
 * Time: 21:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
#if TEST
using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class DigitalChannelInfoTest
	{
		[Test]
		public void ParserTest()
		{
			const string str=@"  4,W8a_KQC C    Off    ,,,0";			
			var channelInfo=new DigitalChannelInformation(str);
			
			Assert.That(channelInfo.index, Is.EqualTo(4));
			Assert.That(channelInfo.name,Is.EqualTo("W8a_KQC C    Off"));
			Assert.That(channelInfo.phase,Is.EqualTo(""));
			Assert.That(channelInfo.circuitComponent,Is.EqualTo(""));
			Assert.That(channelInfo.normalState,Is.EqualTo(false));			
		}
	}
}
#endif
