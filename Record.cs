/*
 * Created by SharpDevelop.
 * User: borisov
 * Date: 24.05.2017
 * Time: 16:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Main class for work with Comtrade
	/// </summary>
	public class Record
	{
		/// <summary>
		/// Get configuration for loaded record
		/// </summary>
		public ConfigurationHandler Configuration{
			get;
			private set;
		}
		
		DataFileHandler data;
		
		internal Record()
		{
		}
		
		/// <summary>
		/// Read record from file
		/// </summary>
		public Record(string fullPathToFile)
		{
			this.OpenFile(fullPathToFile);
		}
		
		internal void OpenFile(string fullPathToFile)
		{			
			string path=System.IO.Path.GetDirectoryName(fullPathToFile);
			string filenameWithoutExtention=System.IO.Path.GetFileNameWithoutExtension(fullPathToFile);
			
			this.Configuration=new ConfigurationHandler(System.IO.Path.Combine(path,filenameWithoutExtention+".cfg"));
			this.data=new DataFileHandler(System.IO.Path.Combine(path,filenameWithoutExtention+".dat"),this.Configuration);
		}
		
		/// <summary>
		/// Get common for all channels set of timestamps
		/// </summary>
		/// <returns>In microSeconds</returns>
		public IReadOnlyList<double> GetTimeLine()
		{
			var list=new double[this.data.samples.Length];
			
			if(this.Configuration.samplingRateCount == 0 || 
			   (Math.Abs(this.Configuration.sampleRates[0].samplingFrequency) < 0.01d)){
				//use timestamps in samples
				for(int i=0;i<this.data.samples.Length;i++){
					list[i]=this.data.samples[i].timestamp*this.Configuration.timeMultiplicationFactor;
				}
			}
			else{//use calculated by samplingFrequency
				double currentTime=0;
				int sampleRateIndex=0;
				for(int i=0;i<this.data.samples.Length;i++){
					list[i]=currentTime;
					if(this.Configuration.sampleRates[sampleRateIndex].lastSampleNumber>=i){
						sampleRateIndex++;
					}					
					currentTime+=0.000001/this.Configuration.sampleRates[sampleRateIndex].samplingFrequency;					
				} 
			}
			
			return list;
		}
		
		/// <summary>
		/// Return sequence of values choosen analog channel
		/// </summary>
		public IReadOnlyList<double> GetAnalogPrimaryChannel(int channelNumber)
		{
			double Kt=1;
			if(this.Configuration.AnalogChannelInformations[channelNumber].isPrimary==false){
				Kt=this.Configuration.AnalogChannelInformations[channelNumber].primary/
					this.Configuration.AnalogChannelInformations[channelNumber].secondary;
			}
			
			var list=new double[this.data.samples.Length];
			for(int i=0;i<this.data.samples.Length;i++){				
				list[i]=(this.data.samples[i].analogs[channelNumber]*this.Configuration.AnalogChannelInformations[channelNumber].a+
				         this.Configuration.AnalogChannelInformations[channelNumber].b)*Kt;
			}
			return list;
		}
		
		/// <summary>
		/// Return sequence of values choosen digital channel
		/// </summary>
		public IReadOnlyList<bool> GetDigitalChannel(int channelNumber)
		{
			var list=new bool[this.data.samples.Length];
			for(int i=0;i<this.data.samples.Length;i++){
				list[i]=this.data.samples[i].digitals[channelNumber];
			}
			return list;
		}
		
	}
}
