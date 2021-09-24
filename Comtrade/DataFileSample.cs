using System;
using System.Collections.Generic;

namespace Wisp.Comtrade
{
	/// <summary>
	/// 
	/// </summary>
	internal class DataFileSample
	{
		public int number;
		
		/// <summary>
		/// microsecond or nanosecond defined by CFG (according STD)
		/// Note: i dont know, where in CFG it is defined, suppose always be microsecond
		/// </summary>
		public int timestamp;
		public double[] analogs;
		public bool[] digitals;
				
		public DataFileSample(int number, int timestamp, double[] analogs, bool[] digitals)
		{
			this.number=number;
			this.timestamp=timestamp;
			this.analogs=analogs;
			this.digitals=digitals;
		}
		
		public DataFileSample(string asciiLine, int analogCount, int digitalCount)
		{
			asciiLine=asciiLine.Replace(GlobalSettings.whiteSpace.ToString(),string.Empty);
			var strings=asciiLine.Split(GlobalSettings.commaDelimiter);
						
			this.analogs=new double[analogCount];
			this.digitals=new bool[digitalCount];				
						
			this.number=Convert.ToInt32(strings[0]);
			this.timestamp=Convert.ToInt32(strings[1]);
			
			for(int i=0;i<analogCount;i++){
				if(strings[i+2]!=string.Empty){//by Standart, can be missing value. In that case by default=0
					this.analogs[i]=Convert.ToDouble(strings[i+2],System.Globalization.CultureInfo.InvariantCulture);
				}
			}
			
			for(int i=0;i<digitalCount;i++){
				this.digitals[i]=Convert.ToBoolean(Convert.ToInt32(strings[i+2+analogCount]));
			}
		}	
		
		public DataFileSample(byte[] bytes, DataFileType dataFileType, int analogCount, int digitalCount)
		{
			this.analogs=new double[analogCount];
			this.digitals=new bool[digitalCount];				
						
			this.number=System.BitConverter.ToInt32(bytes,0);
			this.timestamp=System.BitConverter.ToInt32(bytes,4);
			
			int digitalByteStart;
			if(dataFileType==DataFileType.Binary){
				for(int i=0;i<analogCount;i++){					
					this.analogs[i]=System.BitConverter.ToInt16(bytes,8+i*2);
				}
				digitalByteStart=8+2*analogCount;
			}
			else{
				if(dataFileType==DataFileType.Binary32){
					for(int i=0;i<analogCount;i++){//TODO add test				
						this.analogs[i]=System.BitConverter.ToInt32(bytes,8+i*4);				
					}
				}
				else if(dataFileType==DataFileType.Float32){
					for(int i=0;i<analogCount;i++){//TODO add test			
						this.analogs[i]=System.BitConverter.ToSingle(bytes,8+i*4);				
					}
				}					
				
				digitalByteStart=8+4*analogCount;
			}			
			
			//int digitalByteCount=DataFileHandler.GetDigitalByteCount(digitalCount);
			for(int i=0;i<digitalCount;i++){
				int digitalByteIterator=i/8;
				this.digitals[i]=Convert.ToBoolean((bytes[digitalByteStart+digitalByteIterator]>>(i-digitalByteIterator*8))&1);
			}			
		}
		
		public string ToASCIIDAT()
		{
			string result=string.Empty;
			result+=this.number.ToString();
			result+=GlobalSettings.commaDelimiter+
				this.timestamp.ToString();
			foreach(var analog in this.analogs){
				result+=GlobalSettings.commaDelimiter+
					analog.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			foreach(var digital in this.digitals){
				result+=GlobalSettings.commaDelimiter+
					System.Convert.ToInt32(digital).ToString(System.Globalization.CultureInfo.InvariantCulture);
			}			
			
			return result;
		}
		
		
		public byte[] ToByteDAT(DataFileType dataFileType, IReadOnlyList<AnalogChannelInformation> analogInformations)
		{
			var result=new byte[DataFileHandler.GetByteCount(this.analogs.Length,this.digitals.Length,dataFileType)];
			int analogOneChannelLength= dataFileType == DataFileType.Binary ? 2 : 4;
			int digitalByteStart=8+analogOneChannelLength*this.analogs.Length;
			
			System.BitConverter.GetBytes(this.number).CopyTo(result,0);
			System.BitConverter.GetBytes(this.timestamp).CopyTo(result,4);
			
			switch (dataFileType) {
				case DataFileType.Binary:
					this.AnalogsToBinaryDAT(result, analogInformations);
					break;
				case DataFileType.Binary32:
					this.AnalogsToBinary32DAT(result, analogInformations);
					break;
				case DataFileType.Float32:
					this.AnalogsToFloat32DAT(result);
					break;
				default:
					throw new InvalidOperationException("Not supported file type DAT");
			}
						
			this.DigitalsToDAT(result,digitalByteStart);
			return result;
		}
				
		void AnalogsToBinaryDAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
		{			
			for(int i=0;i<this.analogs.Length;i++){
				short s=(short)((this.analogs[i]-analogInformations[i].b)/analogInformations[i].a);
				System.BitConverter.GetBytes(s).CopyTo(result,8+i*2);
			}			
		}	

		void AnalogsToBinary32DAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
		{			
			for(int i=0;i<this.analogs.Length;i++){
				int s=(int)((this.analogs[i]-analogInformations[i].b)/analogInformations[i].a);
				System.BitConverter.GetBytes(s).CopyTo(result,8+i*4);
			}			
		}		
		
		void AnalogsToFloat32DAT(byte[] result)
		{
			for(int i=0;i<this.analogs.Length;i++){
				System.BitConverter.GetBytes((float)this.analogs[i]).CopyTo(result,8+i*4);
			}		
		}
		
		void DigitalsToDAT(byte[] result, int digitalByteStart)
		{
			int byteIndex=0;
			byte s=0;
			for(int i=0;i<this.digitals.Length;i++){				
				s=(byte)(System.Convert.ToInt32(s)|(System.Convert.ToInt32(this.digitals[i])<<(i-byteIndex*8)));
				
				if((i+1)%8==0 || (i+1)==this.digitals.Length){
					result[digitalByteStart+byteIndex]=s;
					s=0;
					byteIndex++;					
				}
			}
		}
	}
}
