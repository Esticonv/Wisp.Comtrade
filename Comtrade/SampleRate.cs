using System;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Description of SampleRate.
	/// </summary>
	internal class SampleRate
	{
		
		/// <summary>
		/// Hz
		/// </summary>
		readonly public double samplingFrequency=0;
		readonly public int lastSampleNumber=0;
		
		public SampleRate(string sampleRateLine)
		{
			var values=sampleRateLine.Split(GlobalSettings.commaDelimiter);
			this.samplingFrequency=Convert.ToDouble(values[0].Trim(GlobalSettings.whiteSpace),System.Globalization.CultureInfo.InvariantCulture);
			this.lastSampleNumber=Convert.ToInt32(values[1].Trim(GlobalSettings.whiteSpace));
		}
	}
}
