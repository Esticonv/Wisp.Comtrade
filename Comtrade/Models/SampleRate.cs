using System;
using System.Globalization;

namespace Wisp.Comtrade.Models
{
    public class SampleRate
    {
        /// <summary>
        /// Parameter 'samp', Hz
        /// </summary>
		public double SamplingFrequency { get; }

        /// <summary>
        /// Parameter 'endsamp'
        /// </summary>
		public int LastSampleNumber { get; }

        public SampleRate(string sampleRateLine)
        {
            var values = sampleRateLine.Split(GlobalSettings.Comma);
            SamplingFrequency = Convert.ToDouble(values[0].Trim(), CultureInfo.InvariantCulture);
            LastSampleNumber = Convert.ToInt32(values[1].Trim());
        }
    }
}
