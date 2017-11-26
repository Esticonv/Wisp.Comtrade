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
	/// Class for parsing comtrade files
	/// </summary>
	public class RecordReader
	{
		/// <summary>
		/// Get configuration for loaded record
		/// </summary>
		public ConfigurationHandler Configuration{
			get;
			private set;
		}
		
		//DataFileHandler data;		
		
		internal DataFileHandler Data{
			get;
			private set;
		}
		
		internal RecordReader()
		{
		}
		
		/// <summary>
		/// Read record from file
		/// </summary>
		public RecordReader(string fullPathToFile)
		{
			this.OpenFile(fullPathToFile);
		}
		
		internal void OpenFile(string fullPathToFile)
		{			
			string path=System.IO.Path.GetDirectoryName(fullPathToFile);
			string filenameWithoutExtention=System.IO.Path.GetFileNameWithoutExtension(fullPathToFile);
			string extention=System.IO.Path.GetExtension(fullPathToFile).ToLower();
			
			if(extention==GlobalSettings.extentionCFF){
				//TODO доделать cff
				throw new NotImplementedException("*.cff not supported");
			}
			else if(extention==GlobalSettings.extentionCFG || extention==GlobalSettings.extentionDAT){
				this.Configuration=new ConfigurationHandler(System.IO.Path.Combine(path,filenameWithoutExtention+".cfg"));
				this.Data=new DataFileHandler(System.IO.Path.Combine(path,filenameWithoutExtention+".dat"),this.Configuration);
			}
			else{
				throw new InvalidOperationException("Unsupported file extentions. Must be *.cfg, *.dat, *.cff");
			}			
		}
		
		/// <summary>
		/// Get common for all channels set of timestamps
		/// </summary>
		/// <returns>In microSeconds</returns>
		public IReadOnlyList<double> GetTimeLine()
		{
			var list=new double[this.Data.samples.Length];
			
			if(this.Configuration.samplingRateCount == 0 || 
			   (Math.Abs(this.Configuration.sampleRates[0].samplingFrequency) < 0.01d)){
				//use timestamps in samples
				for(int i=0;i<this.Data.samples.Length;i++){
					list[i]=this.Data.samples[i].timestamp*this.Configuration.timeMultiplicationFactor;
				}
			}
			else{//use calculated by samplingFrequency
				double currentTime=0;
				int sampleRateIndex=0;
				const double secondToMicrosecond=1000000;
				for(int i=0;i<this.Data.samples.Length;i++){
					list[i]=currentTime;
					if(i>=this.Configuration.sampleRates[sampleRateIndex].lastSampleNumber){
						sampleRateIndex++;
					}				
					
					currentTime+=secondToMicrosecond/this.Configuration.sampleRates[sampleRateIndex].samplingFrequency;					
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
			
			var list=new double[this.Data.samples.Length];
			for(int i=0;i<this.Data.samples.Length;i++){				
				list[i]=(this.Data.samples[i].analogs[channelNumber]*this.Configuration.AnalogChannelInformations[channelNumber].a+
				         this.Configuration.AnalogChannelInformations[channelNumber].b)*Kt;
			}
			return list;
		}
		
		/// <summary>
		/// Return sequence of values choosen digital channel
		/// </summary>
		public IReadOnlyList<bool> GetDigitalChannel(int channelNumber)
		{
			var list=new bool[this.Data.samples.Length];
			for(int i=0;i<this.Data.samples.Length;i++){
				list[i]=this.Data.samples[i].digitals[channelNumber];
			}
			return list;
		}		
	}
}
