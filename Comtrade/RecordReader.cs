using System;
using System.Collections.Generic;
using System.Linq;
using Wisp.Comtrade.Models;

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
				
		internal DataFileHandler Data{
			get;
			private set;
		}

		/// <summary>
		/// Units for GetTimeLine() 
		/// </summary>
		/// <return>true = ns, false = ms</return>
		public bool TimeLineNanoSecondResolution
        {			
            get {
				if (this.Configuration.SamplingRateCount == 0 || this.Configuration.SampleRates[0].SamplingFrequency == 0) {
					return this.Configuration.TimeLineNanoSecondResolution;
				}
				else {
					return true;
				}
            }
        }
		
		/// <summary>
		/// Read record from file
		/// </summary>
		public RecordReader(string fullPathToFile)
		{
			this.OpenFile(fullPathToFile);
		}

		/// <summary>
		/// Read record from stream
		/// </summary>
		public RecordReader(System.IO.Stream cfgStream, System.IO.Stream datStream)
		{
			this.OpenFromStreamCFG(cfgStream);
			this.OpenFromStreamDAT(datStream);
		}

		/// <summary>
		/// Read record from stream (single file format, *.cff)
		/// </summary>
		public RecordReader(System.IO.Stream cffStream)
		{
			this.OpenFromStreamCFF(cffStream);
		}

		internal void OpenFile(string fullPathToFile)
		{			
			string path=System.IO.Path.GetDirectoryName(fullPathToFile);
			string filenameWithoutExtention=System.IO.Path.GetFileNameWithoutExtension(fullPathToFile);
			string extention=System.IO.Path.GetExtension(fullPathToFile).ToLower();			

			if(extention==GlobalSettings.ExtensionCFF){
				using var cffFileStream = new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtensionCFF, System.IO.FileMode.Open);
				this.OpenFromStreamCFF(cffFileStream);
			}
			else if(extention==GlobalSettings.ExtensionCFG || extention==GlobalSettings.ExtensionDAT){
				using var cfgFileStream= new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtensionCFG, System.IO.FileMode.Open);
				this.OpenFromStreamCFG(cfgFileStream);

				using var datFileStream = new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtensionDAT, System.IO.FileMode.Open);
				this.OpenFromStreamDAT(datFileStream);
			}
			else{
				throw new InvalidOperationException("Unsupported file extentions. Must be *.cfg, *.dat, *.cff");
			}			
		}

		internal void OpenFromStreamCFG(System.IO.Stream cfgStream)
        {
			byte[] buffer = new byte[1024];
			var loadedAsListByteFile = new List<byte>();
			int readedBytes;
			while ((readedBytes = cfgStream.Read(buffer, 0, buffer.Length)) != 0) {
				loadedAsListByteFile.AddRange(buffer.SkipLast(buffer.Length - readedBytes));
			}
			var cfgSection = System.Text.Encoding.UTF8.GetString(loadedAsListByteFile.ToArray())
				.Split(GlobalSettings.NewLines, StringSplitOptions.None);
			this.Configuration = new ConfigurationHandler(cfgSection.ToArray());
		}

		internal void OpenFromStreamDAT(System.IO.Stream datStream)
		{
			byte[] buffer = new byte[1024];
			var loadedAsListByteFile = new List<byte>();
			int readedBytes;
			while ((readedBytes = datStream.Read(buffer, 0, buffer.Length)) != 0) {
				loadedAsListByteFile.AddRange(buffer.SkipLast(buffer.Length - readedBytes));
			}

			if (this.Configuration.DataFileType == DataFileType.ASCII) {
				var datSection = System.Text.Encoding.UTF8.GetString(loadedAsListByteFile.ToArray())
					.Split(GlobalSettings.NewLines, StringSplitOptions.None);
				this.Data = new DataFileHandler(datSection.ToArray(), this.Configuration);
			}
			else {
				this.Data = new DataFileHandler(loadedAsListByteFile.ToArray(), this.Configuration);
			}
		}

		internal void OpenFromStreamCFF(System.IO.Stream cffStream)
		{
			var cfgSection = new List<string>();
			
			byte[] buffer = new byte[1024];
			var loadedAsListByteFile = new List<byte>();
            int readedBytes;
            while ((readedBytes = cffStream.Read(buffer, 0, buffer.Length)) != 0) {
				loadedAsListByteFile.AddRange(buffer.SkipLast(buffer.Length - readedBytes));
			}

			byte[] loadedAsArrayByte = loadedAsListByteFile.ToArray();

			int threeDashCounter = 0;
			int indexOfDataSection = -1;
			for (int i = 2; i < loadedAsListByteFile.Count; i++) {
				if (loadedAsListByteFile[i - 2] == '-' &&
					loadedAsListByteFile[i - 1] == '-' &&
					loadedAsListByteFile[i - 0] == '-') {
					threeDashCounter++;
					if (threeDashCounter == 8) {//4 section header with "--- ......  ---" in each = 8
						indexOfDataSection = i + 3;//skip CRLF(2) and move to next(1) = (2+1) = 3
					}
				}
			}
            if (indexOfDataSection == -1) {
				throw new InvalidOperationException("Not found DAT section, possible incorrect file");
			}

			var cffFileStrings = System.Text.Encoding.UTF8.GetString(loadedAsArrayByte, 0, indexOfDataSection)
				.Split(GlobalSettings.NewLines, StringSplitOptions.None);

			int indexInCff = 0;
			if (!cffFileStrings[indexInCff].Contains("type: CFG")) {
				throw new InvalidOperationException("First line must contains \"file type: CFG\"");
			}
			indexInCff++;
			while (!cffFileStrings[indexInCff].Contains("type: INF")) {
				cfgSection.Add(cffFileStrings[indexInCff]);
				indexInCff++;
			}
			//ignore INF and HDR section
			while (!cffFileStrings[indexInCff].Contains("type: DAT")) {//forward Index to DAT section
				indexInCff++;
			}

			this.Configuration = new ConfigurationHandler(cfgSection.ToArray());
			if (this.Configuration.DataFileType == DataFileType.ASCII) {
				var dataSectionStr=System.Text.Encoding.UTF8.GetString(loadedAsArrayByte, indexOfDataSection, loadedAsArrayByte.Length - indexOfDataSection)
					.Split(GlobalSettings.NewLines, StringSplitOptions.None);

				this.Data = new DataFileHandler(dataSectionStr.ToArray(), this.Configuration);
			}
			else {
				var dataSectionByte = loadedAsArrayByte[indexOfDataSection..];
				this.Data = new DataFileHandler(dataSectionByte, this.Configuration);
			}					
		}

		/// <summary>
		/// Get common for all channels set of timestamps
		/// </summary>
		/// <returns> If guided by samplingFreqency in nanoSecond
		/// Else in microSecond or nanoSecond depending on cfg datatime stamp (6 or 9 Second digit)
		/// Use TimeLineResolution property for get information about</returns>
		public IReadOnlyList<double> GetTimeLine()
		{
			var list=new double[this.Data.Samples.Length];
			
			if(this.Configuration.SamplingRateCount == 0 || 
			   (Math.Abs(this.Configuration.SampleRates[0].SamplingFrequency) < 0.01d)){
				//use timestamps in samples
				for(int i=0;i<this.Data.Samples.Length;i++){
					list[i]=this.Data.Samples[i].Timestamp*this.Configuration.TimeMultiplicationFactor;
				}
			}
			else{//use calculated by samplingFrequency
				double currentTime=0;
				int sampleRateIndex=0;
				const double secondToNanoSecond=1000000000;
				for(int i=0;i<this.Data.Samples.Length;i++){
					list[i]=currentTime;
					if(i>=this.Configuration.SampleRates[sampleRateIndex].LastSampleNumber){
						sampleRateIndex++;
					}				
					
					currentTime+=secondToNanoSecond/this.Configuration.SampleRates[sampleRateIndex].SamplingFrequency;					
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
			if(this.Configuration.AnalogChannelInformationList[channelNumber].IsPrimary==false){
				Kt=this.Configuration.AnalogChannelInformationList[channelNumber].Primary/
					this.Configuration.AnalogChannelInformationList[channelNumber].Secondary;
			}
			
			var list=new double[this.Data.Samples.Length];
			for(int i=0;i<this.Data.Samples.Length;i++){				
				list[i]=(this.Data.Samples[i].AnalogValues[channelNumber]*this.Configuration.AnalogChannelInformationList[channelNumber].MultiplierA+
				         this.Configuration.AnalogChannelInformationList[channelNumber].MultiplierB)*Kt;
			}
			return list;
		}
		
		/// <summary>
		/// Return sequence of values choosen digital channel
		/// </summary>
		public IReadOnlyList<bool> GetDigitalChannel(int channelNumber)
		{
			var list=new bool[this.Data.Samples.Length];
			for(int i=0;i<this.Data.Samples.Length;i++){
				list[i]=this.Data.Samples[i].DigitalValues[channelNumber];
			}
			return list;
		}		
	}
}
