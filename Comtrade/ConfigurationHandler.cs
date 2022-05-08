using System;
using System.Collections.Generic;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Working with *.cfg
	/// </summary>
	public class ConfigurationHandler
	{
		//first line		
		public string StationName=string.Empty;		
		public string DeviceId=string.Empty;
		internal ComtradeVersion version=ComtradeVersion.V1991;//if not presented suppose that v1991
		
		//second line
		internal int analogChannelsCount=0;
		internal int digitalChannelsCount=0;	
		
		
		List<AnalogChannelInformation> analogChannelInformations;		
		public IReadOnlyList<AnalogChannelInformation> AnalogChannelInformations
		{
			get{
				return this.analogChannelInformations;
			}
		}
				
		List<DigitalChannelInformation> digitalChannelInformations;		
		public IReadOnlyList<DigitalChannelInformation> DigitalChannelInformations
		{
			get{
				return this.digitalChannelInformations;
			}
		}
		
		public double frequency=50.0;
		
		internal int samplingRateCount=0;
		internal List<SampleRate> sampleRates;
		
		/// <summary>
		/// Time of first value in data
		/// </summary>
		public DateTime StartTime{get;private set;}
		
		/// <summary>
		/// Time of trigger point
		/// </summary>
		public DateTime TriggerTime{get;private set;}
						
		internal DataFileType dataFileType=DataFileType.Undefined;	

		internal double timeMultiplicationFactor=1.0;

		/// <summary>
		/// used for test case
		/// </summary>
		internal ConfigurationHandler()
		{
			
		}
		
		internal ConfigurationHandler(string fullPathToFileCFG)
		{
			this.Parse(System.IO.File.ReadAllLines(fullPathToFileCFG, System.Text.Encoding.Default));
		}

		internal ConfigurationHandler(string[] strings)
		{
			this.Parse(strings);
		}

		internal void Parse(string[] strings)
		{
			this.ParseFirstLine(strings[0]);
			this.ParseSecondLine(strings[1]);
			
			this.analogChannelInformations=new List<AnalogChannelInformation>();
			for(int i=0;i<this.analogChannelsCount;i++){
				this.analogChannelInformations.Add(new AnalogChannelInformation(strings[2+i]));
			}
			
			this.digitalChannelInformations=new List<DigitalChannelInformation>();
			for(int i=0;i<this.digitalChannelsCount;i++){
				this.digitalChannelInformations.Add(new DigitalChannelInformation(strings[2+i+this.analogChannelsCount]));
			}
			
			var strIndex=2+this.analogChannelsCount+this.digitalChannelsCount;
			this.ParseFrequenceLine(strings[strIndex++]);
			
			this.ParseNumberOfSampleRates(strings[strIndex++]);
			
			this.sampleRates=new List<SampleRate>();
			if(this.samplingRateCount==0){
				this.sampleRates.Add(new SampleRate(strings[strIndex++]));
			}
			else{
				for(int i=0;i<this.samplingRateCount;i++){
					this.sampleRates.Add(new SampleRate(strings[strIndex+i]));
				}
				strIndex+=this.samplingRateCount;
			}
						
			this.StartTime=ParseDateTime(strings[strIndex++]);
			this.TriggerTime=ParseDateTime(strings[strIndex++]);			
			
			this.ParseDataFileType(strings[strIndex++]);
			
			this.ParseTimeMultiplicationFactor(strings[strIndex++]);

			//TODO add non-essential fields for standart 2013
		}

		void ParseFirstLine(string firstLine)
		{			
			firstLine=firstLine.Replace(GlobalSettings.whiteSpace.ToString(),string.Empty);
			var values=firstLine.Split(GlobalSettings.commaDelimiter);
			this.StationName=values[0];
			this.DeviceId=values[1];
			if(values.Length==3){
				this.version=ComtradeVersionConverter.Get(values[2]);
			}			
		}
		
		void ParseSecondLine(string secondLine)
		{	
			secondLine=secondLine.Replace(GlobalSettings.whiteSpace.ToString(),string.Empty);
			var values=secondLine.Split(GlobalSettings.commaDelimiter);
			//values[0];//not used, is equal sum of two next
			this.analogChannelsCount=Convert.ToInt32(values[1].TrimEnd('A'), System.Globalization.CultureInfo.InvariantCulture);
			this.digitalChannelsCount=Convert.ToInt32(values[2].TrimEnd('D'), System.Globalization.CultureInfo.InvariantCulture);
		}
		
		void ParseFrequenceLine(string frequenceLine)
		{
			this.frequency=Convert.ToDouble(frequenceLine.Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
		}
		
		void ParseNumberOfSampleRates(string str)
		{
			this.samplingRateCount=Convert.ToInt32(str.Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);			
		}
		
		internal static DateTime ParseDateTime(string str)
		{	// "dd/mm/yyyy,hh:mm:ss.ssssss"			
			DateTime.TryParseExact(str,GlobalSettings.dateTimeFormat,
			                       System.Globalization.CultureInfo.InvariantCulture,
			                       System.Globalization.DateTimeStyles.AllowWhiteSpaces,
			                       out DateTime result);
			return result;
		}
		
		void ParseDataFileType(string str)
		{
			this.dataFileType=DataFileTypeConverter.Get(str.Trim(GlobalSettings.whiteSpace));
		}
		
		void ParseTimeMultiplicationFactor(string str)
		{
			this.timeMultiplicationFactor=Convert.ToDouble(str.Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}