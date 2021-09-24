/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 23.05.2017
 * Time: 22:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class SampleRateTest
	{
		[Test]
		public void ParseTest()
		{
			const string str=@" 0 ,  1360";
			var sampleRate=new SampleRate(str);
			
			Assert.That(sampleRate.samplingFrequency,Is.EqualTo(0).Within(0.1));
			Assert.That(sampleRate.lastSampleNumber,Is.EqualTo(1360));
		}
	}
}

