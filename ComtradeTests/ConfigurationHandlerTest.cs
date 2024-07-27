using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Wisp.Comtrade
{
	[TestClass]
	public class ConfigurationHandlerTest
	{
		[TestMethod]
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
			var configHandler=new ConfigurationHandler(strings);

			Assert.AreEqual("MASHUK-W2D-C60-1",		configHandler.StationName);
			Assert.AreEqual("520",					configHandler.DeviceId);
			Assert.AreEqual(ComtradeVersion.V1999,	configHandler.Version);
			Assert.AreEqual(2,						configHandler.AnalogChannelsCount);
			Assert.AreEqual(2,						configHandler.AnalogChannelInformationList.Count);
			Assert.AreEqual(3,						configHandler.DigitalChannelsCount);
			Assert.AreEqual(3,						configHandler.DigitalChannelInformationList.Count);
			Assert.AreEqual(50,						configHandler.Frequency, 0.01) ;
			Assert.AreEqual(0,						configHandler.SamplingRateCount);
			Assert.AreEqual(1,						configHandler.SampleRates.Count);
			//Assert.AreEqual(, ); время1
			//Assert.AreEqual(, ); время2
			Assert.AreEqual(DataFileType.Binary, configHandler.DataFileType);
			//TODO add other field asserts
		}
	}
}

