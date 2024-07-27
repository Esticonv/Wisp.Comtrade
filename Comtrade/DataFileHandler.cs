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
			return (digitalChannelsCount/16+((digitalChannelsCount%16) == 0 ? 0:1))*DataFileHandler.digital16ChannelLength;
		}
		
		internal static int GetByteCountInOneSample(int analogsChannelsCount, int digitalChannelsCount, DataFileType dataFileType)
		{
			int analogOneChannelLength = dataFileType == DataFileType.Binary ? 2 : 4;
			return  DataFileHandler.sampleNumberLength +
					DataFileHandler.timeStampLength +
					analogsChannelsCount*analogOneChannelLength +
				    DataFileHandler.GetDigitalByteCount(digitalChannelsCount);
		}

		internal DataFileHandler(string[] strings, ConfigurationHandler configuration)
		{
			int samplesCount = configuration.SampleRates[^1].lastSampleNumber;
			this.samples = new DataFileSample[samplesCount];

			if (configuration.DataFileType == DataFileType.ASCII) {
				strings = strings.Where(x => x != string.Empty).ToArray();//removing empty strings (when *.dat file not following Standart)
				for (int i = 0; i < samplesCount; i++) {
					this.samples[i] = new DataFileSample(strings[i],
													   configuration.AnalogChannelsCount, configuration.DigitalChannelsCount);
				}
			}
            else {
				throw new InvalidOperationException($"Configuration dataFileType must be ASCII, but was {configuration.DataFileType}");
            }
		}

		internal DataFileHandler(byte[] bytes, ConfigurationHandler configuration)
		{
			int samplesCount = configuration.SampleRates[^1].lastSampleNumber;
			this.samples = new DataFileSample[samplesCount];

			if (configuration.DataFileType == DataFileType.Binary ||
			   configuration.DataFileType == DataFileType.Binary32 ||
			   configuration.DataFileType == DataFileType.Float32) {
				//var fileContent = System.IO.File.ReadAllBytes(fullPathToFileDAT);

				int oneSampleLength = DataFileHandler.GetByteCountInOneSample(configuration.AnalogChannelsCount,
																 configuration.DigitalChannelsCount,
																 configuration.DataFileType);

				for (int i = 0; i < samplesCount; i++) {
					var bytesOneSample = new byte[oneSampleLength];
					for (int j = 0; j < oneSampleLength; j++) {
						bytesOneSample[j] = bytes[i * oneSampleLength + j];
					}
					this.samples[i] = new DataFileSample(bytesOneSample, configuration.DataFileType,
													   configuration.AnalogChannelsCount, configuration.DigitalChannelsCount);
				}
			}
			else {
				throw new InvalidOperationException($"Configuration dataFileType must be Binary, Binary32 or Float , but was {configuration.DataFileType}");
			}
		
		}
	}
}
