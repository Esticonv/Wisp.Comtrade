/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 23.05.2017
 * Time: 21:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class ConfigurationHandlerTest
	{
		[Test]
		public void ParserTest()
		{
			const string str=@"MASHUK-W2D-C60-1    ,520                 ,1999
 5,2A, 3D
  1,F1-IA               ,A,,A     ,     0.001953,0,0,-32767,32767,   1000.0,  1.0,S
  2,F2-IB               ,B,,A     ,     0.001953,0,0,-32767,32767,   1000.0,  1.0,S
    1,OSC TRIG     On     ,,,0
  2,W7a_KQC A    Off    ,,,0
  3,W7c_KQC B    Off    ,,,0
50
0
0,  1360
06/04/2012,06:42:59.391076
06/04/2012,06:42:59.895690
BINARY
1.00
";
			var strings=str.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			var configHandler=new ConfigurationHandler();
			configHandler.Parse(strings);
			
			Assert.That(configHandler.StationName,Is.EqualTo("MASHUK-W2D-C60-1"));
			Assert.That(configHandler.DeviceId,Is.EqualTo("520"));
			Assert.That(configHandler.version,Is.EqualTo(ComtradeVersion.V1999));
			Assert.That(configHandler.analogChannelsCount,Is.EqualTo(2));
			Assert.That(configHandler.AnalogChannelInformations.Count,Is.EqualTo(2));
			Assert.That(configHandler.digitalChannelsCount,Is.EqualTo(3));
			Assert.That(configHandler.DigitalChannelInformations.Count,Is.EqualTo(3));
			Assert.That(configHandler.frequency,Is.EqualTo(50).Within(0.1));
			Assert.That(configHandler.samplingRateCount,Is.EqualTo(0));
			Assert.That(configHandler.sampleRates.Count,Is.EqualTo(1));
			//время1
			//время2
			Assert.That(configHandler.dataFileType,Is.EqualTo(DataFileType.Binary));
			//остальное дописать
		}
	}
}

