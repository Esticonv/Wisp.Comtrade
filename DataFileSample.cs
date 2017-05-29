/*
 * Created by SharpDevelop.
 * User: borisov
 * Date: 24.05.2017
 * Time: 9:02
 */
using System;

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
					for(int i=0;i<analogCount;i++){//TODO добавить тест				
						this.analogs[i]=System.BitConverter.ToInt32(bytes,8+i*4);				
					}
				}
				else if(dataFileType==DataFileType.Float32){
					for(int i=0;i<analogCount;i++){//TODO добавить тест				
						this.analogs[i]=System.BitConverter.ToSingle(bytes,8+i*4);				
					}
				}					
				
				digitalByteStart=8+4*analogCount;
			}			
			
			int digitalByteCount=DataFileHandler.GetDigitalByteCount(digitalCount);
			for(int i=0;i<digitalCount;i++){
				int digitalByteIterator=i/8;
				this.digitals[i]=Convert.ToBoolean((bytes[digitalByteStart+digitalByteIterator]>>(i-digitalByteIterator*8))&1);
			}			
		}
	}
}
