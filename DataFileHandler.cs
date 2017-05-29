/*
 * Created by SharpDevelop.
 * User: borisov
 * Date: 24.05.2017
 * Time: 8:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Description of DataFileHandler.
	/// </summary>
	internal class DataFileHandler
	{
		internal DataFileSample[] samples;
		
		internal const int sampleNumberLength=4;
		internal const int timeStampLength=4;
		internal const int digital16ChannelLength=2;
		
		internal static int GetDigitalByteCount(int digitalChannelsCount)
		{
			return
				(digitalChannelsCount/16+((digitalChannelsCount%16) == 0 ? 0:1))*DataFileHandler.digital16ChannelLength;
		}
		
		internal DataFileHandler(string fullPathToFileDAT, ConfigurationHandler configuration)
		{
			int samplesCount=configuration.sampleRates[configuration.sampleRates.Count-1].lastSampleNumber;	
			this.samples=new DataFileSample[samplesCount];
			
			if(configuration.dataFileType==DataFileType.Binary ||
			   configuration.dataFileType==DataFileType.Binary32 ||
			   configuration.dataFileType==DataFileType.Float32){
				var fileContent=System.IO.File.ReadAllBytes(fullPathToFileDAT);
				
				//count of bytes (with 8 bit each)				
				int analogOneChannelLength=configuration.dataFileType == DataFileType.Binary ? 2 : 4;
				
				int oneSampleLength=	DataFileHandler.sampleNumberLength+
										DataFileHandler.timeStampLength+
										configuration.analogChannelsCount*analogOneChannelLength+
										DataFileHandler.GetDigitalByteCount(configuration.digitalChannelsCount);					
				
				
				for(int i=0;i<samplesCount;i++){
					var bytes=new byte[oneSampleLength];
					for(int j=0;j<oneSampleLength;j++){
						bytes[j]=fileContent[i*oneSampleLength+j];
					}
					this.samples[i]=new DataFileSample(bytes, configuration.dataFileType,
					                                   configuration.analogChannelsCount, configuration.digitalChannelsCount);
				}
				
			}
			else if(configuration.dataFileType==DataFileType.ASCII){
				var strings=System.IO.File.ReadAllLines(fullPathToFileDAT);
				strings=strings.Where(x => x != string.Empty).ToArray();//removing empty strings (when *.dat file not following Standart)
				for(int i=0;i<samplesCount;i++){
					this.samples[i]=new DataFileSample(strings[i],
					                                   configuration.analogChannelsCount, configuration.digitalChannelsCount);
				}
			}
		}
	}
}
