using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Wisp.Comtrade
{
	[TestClass]
	public class SampleRateTest
	{
		[TestMethod]
		public void ParseTest()
		{
			const string str=@" 0 ,  1360";
			var sampleRate=new SampleRate(str);

			Assert.AreEqual(0,		sampleRate.SamplingFrequency, 0.1);
			Assert.AreEqual(1360,	sampleRate.LastSampleNumber);
		}
	}
}

