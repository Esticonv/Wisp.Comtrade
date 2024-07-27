using System;
using System.Collections.Generic;
using System.Linq;

namespace Wisp.Comtrade
{
    /// <summary>
    /// For creating COMTRADE files
    /// </summary>
    public class RecordWriter
	{
		/// <summary>
		/// Hz
		/// </summary>
		public double Frequency = 50;
		public string StationName=string.Empty;	
		public string DeviceId=string.Empty;
		
		readonly List<DataFileSample> samples=new();
		readonly List<AnalogChannelInformation> analogChannelInformations=new();
		readonly List<DigitalChannelInformation> digitalChannelInformations=new();
		readonly List<SampleRate> sampleRates=new();

		
		/// <summary>
		/// Time of first value in data
		/// </summary>
		public DateTime StartTime;
		
		/// <summary>
		/// Time of trigger point
		/// </summary>
		public DateTime TriggerTime;
		
		/// <summary>
		/// Create empty writer
		/// </summary>
		public RecordWriter()
		{
		}
		
		/// <summary>
		/// Create writer with data from reader
		/// </summary>
		public RecordWriter(RecordReader reader)
		{
			this.StationName=reader.Configuration.StationName;
			this.DeviceId=reader.Configuration.DeviceId;

			this.samples.AddRange(reader.Data.samples);
			this.analogChannelInformations.AddRange(reader.Configuration.AnalogChannelInformations);
			this.digitalChannelInformations.AddRange(reader.Configuration.DigitalChannelInformations);
			this.sampleRates.AddRange(reader.Configuration.sampleRates);
			
			this.StartTime=reader.Configuration.StartTime;
			this.TriggerTime=reader.Configuration.TriggerTime;
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
		/// <param name="timestamp">In nanosecond, but can be in microsecond depending from CFG</param>
		/// <param name="analogs"></param>
		/// <param name="digitals"></param>
		public void AddSample(int timestamp, double[] analogs, bool[] digitals)
		{			
			if(this.analogChannelInformations.Count!= analogs.Length){
				throw new InvalidOperationException(
					$"Analogs count ({ analogs.Length }) must be equal to channels count ({this.analogChannelInformations.Count})");
			}
			
			if(this.digitalChannelInformations.Count!= digitals.Length){
				throw new InvalidOperationException(
					$"Digitals count ({ digitals.Length }) must be equal to channels count ({this.digitalChannelInformations.Count})");
			}
			
			this.samples.Add(new DataFileSample(this.samples.Count+1,timestamp, analogs, digitals));
		}

		/// <summary>
		/// Support only Ascii or Binary file type
		/// </summary>
		public void SaveToFile(string fullPathToFile, bool singleFile, DataFileType dataFileType = DataFileType.Binary)
		{
			if (dataFileType == DataFileType.Undefined ||
			   dataFileType == DataFileType.Binary32 ||
			   dataFileType == DataFileType.Float32) {
				throw new InvalidOperationException($"dataFileType={dataFileType} currently unsupported");
			}

			string path = System.IO.Path.GetDirectoryName(fullPathToFile);
			string filenameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(fullPathToFile);

			if (singleFile) {
				using var fileStreamCFF = new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtentionCFF, System.IO.FileMode.Create);
				this.SaveToStreamCFGSection(fileStreamCFF, singleFile, dataFileType);
				this.SaveToStreamDATSection(fileStreamCFF, singleFile, dataFileType);
			}
            else {
				using var fileStreamCFG = new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtentionCFG, System.IO.FileMode.Create);
				this.SaveToStreamCFGSection(fileStreamCFG, singleFile, dataFileType);

				using var fileStreamDAT = new System.IO.FileStream(System.IO.Path.Combine(path, filenameWithoutExtention) + GlobalSettings.ExtentionDAT, System.IO.FileMode.Create);
				this.SaveToStreamDATSection(fileStreamDAT, singleFile, dataFileType);
			}

		}

		public void SaveToStreamCFGSection(System.IO.Stream stream, bool singleFile, DataFileType dataFileType)
		{
			this.CalculateScaleFactorAB(dataFileType);

			var strings = new List<string>();

			if (singleFile == true) {
				strings.Add("--- file type: CFG ---");
			}

			strings.Add(this.StationName + GlobalSettings.Comma +
						this.DeviceId + GlobalSettings.Comma +
						"2013");

			strings.Add((this.analogChannelInformations.Count + this.digitalChannelInformations.Count).ToString() + GlobalSettings.Comma +
						this.analogChannelInformations.Count.ToString() + "A" + GlobalSettings.Comma +
						this.digitalChannelInformations.Count.ToString() + "D");

			for (int i = 0; i < this.analogChannelInformations.Count; i++) {
				strings.Add(this.analogChannelInformations[i].ToCFGString());
			}

			for (int i = 0; i < this.digitalChannelInformations.Count; i++) {
				strings.Add(this.digitalChannelInformations[i].ToCFGString());
			}

			strings.Add(this.Frequency.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.sampleRates == null || this.sampleRates.Count == 0) {
				strings.Add("0");
				strings.Add("0" + GlobalSettings.Comma +
							this.samples.Count.ToString());
			}
			else {
				strings.Add(this.sampleRates.Count.ToString());
				foreach (var sampleRate in this.sampleRates) {
					strings.Add(sampleRate.samplingFrequency.ToString() + GlobalSettings.Comma +
								sampleRate.lastSampleNumber.ToString());
				}
			}

			strings.Add(this.StartTime.ToString(GlobalSettings.DateTimeFormatForWrite,
								   System.Globalization.CultureInfo.InvariantCulture));

			strings.Add(this.TriggerTime.ToString(GlobalSettings.DateTimeFormatForWrite,
								   System.Globalization.CultureInfo.InvariantCulture));

			switch (dataFileType) {
				case DataFileType.ASCII:
					strings.Add("ASCII");
					break;
				case DataFileType.Binary:
					strings.Add("BINARY");
					break;
				case DataFileType.Binary32:
					strings.Add("BINARY32");
					break;
				case DataFileType.Float32:
					strings.Add("FLOAT32");
					break;
				default:
					throw new InvalidOperationException("Undefined data file type =" + dataFileType.ToString());
			}

			strings.Add("1.0");

            if (singleFile) {
				strings.Add("--- file type: INF ---");
				strings.Add(GlobalSettings.NewLine);
				strings.Add("--- file type: HDR ---");
				strings.Add(GlobalSettings.NewLine);
			}

			foreach (var str in strings.SkipLast(1)) {
				stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
				if (str != GlobalSettings.NewLine) {
					stream.Write(System.Text.Encoding.UTF8.GetBytes(GlobalSettings.NewLine));
				}
			}
			stream.Write(System.Text.Encoding.UTF8.GetBytes(strings[^1]));
		}

		public void SaveToStreamDATSection(System.IO.Stream stream, bool singleFile, DataFileType dataFileType)
        {
			var strings = new List<string>();
            if (singleFile) {
				if (dataFileType == DataFileType.ASCII) {
					strings.Add("--- file type: DAT ASCII ---");
					foreach (var sample in this.samples) {
						strings.Add(sample.ToASCIIDAT());
					}

					foreach (var str in strings.SkipLast(1)) {
						stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
						stream.Write(System.Text.Encoding.UTF8.GetBytes(GlobalSettings.NewLine));
					}
					stream.Write(System.Text.Encoding.UTF8.GetBytes(strings[^1]));
				}
				else {//binary
					int byteInOneSample = DataFileHandler.GetByteCountInOneSample(this.analogChannelInformations.Count, this.digitalChannelInformations.Count, dataFileType);
					strings.Add($"--- file type: DAT BINARY: {byteInOneSample * this.samples.Count} ---");
					foreach (var str in strings) {
						stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
						stream.Write(System.Text.Encoding.UTF8.GetBytes(GlobalSettings.NewLine));
					}

					foreach (var sample in this.samples) {
						stream.Write(sample.ToByteDAT(dataFileType, this.analogChannelInformations));
					}
				}
			}
			else {
				if (dataFileType == DataFileType.ASCII) {
					foreach (var sample in this.samples) {
						strings.Add(sample.ToASCIIDAT());
					}
					foreach (var str in strings.SkipLast(1)) {
						stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
						stream.Write(System.Text.Encoding.UTF8.GetBytes(GlobalSettings.NewLine));
					}
					stream.Write(System.Text.Encoding.UTF8.GetBytes(strings[^1]));
				}
				else {//binary
					foreach (var sample in this.samples) {
						stream.Write(sample.ToByteDAT(dataFileType, this.analogChannelInformations));
					}					
				}
			}
		}		
		
		void CalculateScaleFactorAB(DataFileType dataFileType)
		{
			if(dataFileType==DataFileType.Binary ||
			   dataFileType==DataFileType.Binary32){//i make it same, but in theory, bin32 can be more precise			
				for(int i=0;i<this.analogChannelInformations.Count;i++){
					double min=this.samples.Min(x => x.AnalogValues[i]);
					double max=this.samples.Max(x => x.AnalogValues[i]);					
					this.analogChannelInformations[i].MultiplierB=(max+min)/2.0;
					if(max!=min){
						this.analogChannelInformations[i].MultiplierA = (max-min)/32767.0;//65536						
					}					
					this.analogChannelInformations[i].Min=-32767;//by standart 1999
					this.analogChannelInformations[i].Max=32767;//by standart 1999					
				}				
			}			
			else if(dataFileType==DataFileType.ASCII){
				foreach(var analogChannelInformation in this.analogChannelInformations){
					analogChannelInformation.Min=-32767;//by standart 1999
					analogChannelInformation.Max=32767;//by standart 1999
				}
			}			
		}		
	}
}
