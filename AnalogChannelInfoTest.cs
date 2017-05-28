/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 23.05.2017
 * Time: 21:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
#if TEST
using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class AnalogChannelInfoTest
	{
		[Test]
		public void ParserTest()
		{
			const string str=@"  8,F8-VN               ,N,,V     ,     0.012207,0,0,-32767,32767, 330000.0,100.0,S";			
			var channelInfo=new AnalogChannelInformation(str);
			
			Assert.That(channelInfo.index, Is.EqualTo(8));
			Assert.That(channelInfo.name,Is.EqualTo("F8-VN"));
			Assert.That(channelInfo.phase,Is.EqualTo("N"));
			Assert.That(channelInfo.circuitComponent,Is.EqualTo(""));
			Assert.That(channelInfo.units,Is.EqualTo("V"));
			Assert.That(channelInfo.a,Is.EqualTo(0.012207).Within(0.001));
			Assert.That(channelInfo.b,Is.EqualTo(0).Within(0.001));
			Assert.That(channelInfo.skew,Is.EqualTo(0).Within(0.001));
			Assert.That(channelInfo.min,Is.EqualTo(-32767).Within(0.001));
			Assert.That(channelInfo.max,Is.EqualTo(32767).Within(0.001));
			Assert.That(channelInfo.primary,Is.EqualTo(330000.0).Within(0.001));
			Assert.That(channelInfo.secondary,Is.EqualTo(100.0).Within(0.001));
			Assert.That(channelInfo.isPrimary,Is.EqualTo(false));	
		}
	}
}
#endif
