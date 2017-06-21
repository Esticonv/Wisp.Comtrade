/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 07.06.2017
 * Time: 12:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wisp.Comtrade
{
	/// <summary>
	/// For creating COMTRADE files
	/// Currently, supported only one mode COMTRADE: binary, timestamp guided
	/// </summary>
	public class RecordWriter
	{
		string stationName=string.Empty;
		string deviceId=string.Empty;
		
		List<DataFileSample> samples;
		List<AnalogChannelInformation> analogChannelInformations;			
		List<DigitalChannelInformation> digitalChannelInformations;
		
		/// <summary>
		/// 
		/// </summary>
		public RecordWriter()
		{
			this.samples=new List<DataFileSample>();
			this.analogChannelInformations=new List<AnalogChannelInformation>();
			this.digitalChannelInformations=new List<DigitalChannelInformation>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void AddAnalogChannel(AnalogChannelInformation analogChannel)
		{
			analogChannel.Index=this.analogChannelInformations.Count+1;
			this.analogChannelInformations.Add(analogChannel);			
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void AddDigitalChannel(DigitalChannelInformation digitalChannel)
		{
			digitalChannel.Index=this.digitalChannelInformations.Count+1;
			this.digitalChannelInformations.Add(digitalChannel);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timestamp">micro second</param>
		/// <param name="analogs"></param>
		/// <param name="digitals"></param>
		public void AddSample(int timestamp, double[] analogs, bool[] digitals)
		{			
			double[] notNullAnalogs=analogs;
			bool[] notNullDigitals=digitals;
			if(analogs==null){
				notNullAnalogs=new double[0];
			}
			if(digitals==null){
				notNullDigitals=new bool[0];
			}
			
			if(this.analogChannelInformations.Count!=notNullAnalogs.Length){
				throw new InvalidOperationException(string.Format("Analogs count ({0}) must be equal to channels count ({1})",
				                                                  notNullAnalogs.Length,
				                                                  this.analogChannelInformations.Count));
			}
			
			if(this.digitalChannelInformations.Count!=notNullDigitals.Length){
				throw new InvalidOperationException(string.Format("Digitals count ({0}) must be equal to channels count ({1})",
				                                                  notNullDigitals.Length,
				                                                  this.digitalChannelInformations.Count));
			}
			
			this.samples.Add(new DataFileSample(this.samples.Count+1,timestamp,notNullAnalogs,notNullDigitals));
		}
		
		/// <summary>
		/// Support only Ascii or Binary file type
		/// </summary>
		public void SaveToFile(string fullPathToFile, DataFileType dataFileType=DataFileType.Binary)
		{	
			if(dataFileType==DataFileType.Undefined ||
			   dataFileType==DataFileType.Binary32 ||
			   dataFileType==DataFileType.Float32){
				throw new InvalidOperationException("Currently unsupported "+dataFileType.ToString());
			}
			
			
			string path=System.IO.Path.GetDirectoryName(fullPathToFile);
			string filenameWithoutExtention=System.IO.Path.GetFileNameWithoutExtension(fullPathToFile);
			
			this.CalculateScaleFactorAB(dataFileType);
			
			//CFG part
			var strings=new List<string>();
			
			strings.Add(this.stationName+GlobalSettings.commaDelimiter+
			            this.deviceId+GlobalSettings.commaDelimiter+
			            "1999");
			
			strings.Add((this.analogChannelInformations.Count+this.digitalChannelInformations.Count).ToString()+GlobalSettings.commaDelimiter+
			            this.analogChannelInformations.Count.ToString()+"A"+GlobalSettings.commaDelimiter+
			            this.digitalChannelInformations.Count.ToString()+"D");
			
			for(int i=0;i<this.analogChannelInformations.Count;i++){
				strings.Add(this.analogChannelInformations[i].ToCFGString());
			}
			
			for(int i=0;i<this.digitalChannelInformations.Count;i++){
				strings.Add(this.digitalChannelInformations[i].ToCFGString());
			}
			
			strings.Add("50.0");
			
			strings.Add("0");
			
			strings.Add("0"+GlobalSettings.commaDelimiter+
			            this.samples.Count.ToString());
			
			strings.Add("01/01/2017"+GlobalSettings.commaDelimiter+
			            "00:00:00.000000");
			
			strings.Add("01/01/2017"+GlobalSettings.commaDelimiter+
			            "00:00:00.000001");
			
			
			switch (dataFileType) {
				case DataFileType.ASCII:    strings.Add("ASCII"); break;
				case DataFileType.Binary:   strings.Add("BINARY");  break;
				case DataFileType.Binary32: strings.Add("BINARY32");  break; 
				case DataFileType.Float32:  strings.Add("FLOAT32");  break;
				default:
					throw new InvalidOperationException("Undefined data file type ="+dataFileType.ToString());					
			}			
			
			strings.Add("1.0");
			
			System.IO.File.WriteAllLines(System.IO.Path.Combine(path,filenameWithoutExtention)+GlobalSettings.extentionCFG, strings);
			
			//DAT part
			string dataFileFullPath=System.IO.Path.Combine(path,filenameWithoutExtention)+GlobalSettings.extentionDAT;
			
			if(dataFileType==DataFileType.ASCII){
				strings=new List<string>();
				foreach(var sample in this.samples){
					strings.Add(sample.ToASCIIDAT());
				}				
				System.IO.File.WriteAllLines(dataFileFullPath, strings);
			}
			else{
				var bytes=new List<byte>();				
				foreach(var sample in this.samples){
					bytes.AddRange(sample.ToByteDAT(dataFileType,this.analogChannelInformations));
				}				
				System.IO.File.WriteAllBytes(dataFileFullPath, bytes.ToArray());
			}
		}
		
		void CalculateScaleFactorAB(DataFileType dataFileType)
		{
			if(dataFileType==DataFileType.Binary ||
			   dataFileType==DataFileType.Binary32){//i make it same, but in theory, bin32 can be more precise			
				for(int i=0;i<this.analogChannelInformations.Count;i++){
					double min=this.samples.Min(x => x.analogs[i]);
					double max=this.samples.Max(x => x.analogs[i]);					
					this.analogChannelInformations[i].b=(max+min)/2.0;
					if(max!=min){
						this.analogChannelInformations[i].a=(max-min)/32767.0;//65536						
					}					
					this.analogChannelInformations[i].Min=-32767;//by standart 1999
					this.analogChannelInformations[i].Max=32767;//by standart 1999					
				}				
			}
			
			if(dataFileType==DataFileType.ASCII){
				foreach(var analogChannelInformation in this.analogChannelInformations){
					analogChannelInformation.Min=-32767;//by standart 1999
					analogChannelInformation.Max=32767;//by standart 1999
				}
			}			
		}
		
	}
}
