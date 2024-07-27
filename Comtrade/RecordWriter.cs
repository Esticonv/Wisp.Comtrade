using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Wisp.Comtrade.Models;

namespace Wisp.Comtrade;

/// <summary>
///     For creating COMTRADE files
/// </summary>
public class RecordWriter
{
    private readonly List<AnalogChannel> _analogChannels = [];
    private readonly string _deviceId = string.Empty;
    private readonly List<DigitalChannel> _digitalChannels = [];
    private readonly double _frequency = 50;
    private readonly List<SampleRate> _sampleRates = [];
    private readonly List<DataFileSample> _samples = [];
    private readonly string _stationName = string.Empty;

    /// <summary>
    ///     Create empty writer
    /// </summary>
    public RecordWriter()
    {
    }

    /// <summary>
    ///     Create writer with data from reader
    /// </summary>
    public RecordWriter(RecordReader reader)
    {
        if (reader.Data is null || reader.Configuration is null)
            return;

        _stationName = reader.Configuration.StationName;
        _deviceId = reader.Configuration.DeviceId;
        _frequency = reader.Configuration.Frequency;

        _samples.AddRange(reader.Data.Samples);
        _analogChannels.AddRange(reader.Configuration.AnalogChannelInformationList);
        _digitalChannels.AddRange(reader.Configuration.DigitalChannelInformationList);
        _sampleRates.AddRange(reader.Configuration.SampleRates);

        StartTime = reader.Configuration.StartTime;
        TriggerTime = reader.Configuration.TriggerTime;
    }

    /// <summary>
    ///     Time of first value in data
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     Time of trigger point
    /// </summary>
    public DateTime TriggerTime { get; set; }

    /// <summary>
    /// </summary>
    public void AddAnalogChannel(AnalogChannel analogChannel)
    {
        analogChannel.Index = _analogChannels.Count + 1;
        _analogChannels.Add(analogChannel);
    }

    /// <summary>
    /// </summary>
    public void AddDigitalChannel(DigitalChannel digitalChannel)
    {
        digitalChannel.Index = _digitalChannels.Count + 1;
        _digitalChannels.Add(digitalChannel);
    }

    /// <summary>
    /// </summary>
    /// <param name="timestamp">In microsecond or nanosecond depending on confgiruation</param>
    /// <param name="analogValues"></param>
    /// <param name="digitalValues"></param>
    public void AddSample(int timestamp, double[] analogValues, bool[] digitalValues)
    {
        if (_analogChannels.Count != analogValues.Length) {
            throw new InvalidOperationException(
                $"Analogs count ({analogValues.Length}) must be equal to channels count ({_analogChannels.Count})");
        }

        if (_digitalChannels.Count != digitalValues.Length) {
            throw new InvalidOperationException(
                $"Digitals count ({digitalValues.Length}) must be equal to channels count ({_digitalChannels.Count})");
        }

        _samples.Add(new DataFileSample(_samples.Count + 1, timestamp, analogValues, digitalValues));
    }

    /// <summary>
    ///     Support only Ascii or Binary file type
    /// </summary>
    public void SaveToFile(string fullPathToFile, bool singleFile, DataFileType dataFileType = DataFileType.Binary)
    {
        if (dataFileType is DataFileType.Undefined or DataFileType.Binary32 or DataFileType.Float32) {
            throw new InvalidOperationException($"dataFileType={dataFileType} currently unsupported");
        }

        var fileInfo = new FileInfo(fullPathToFile);
        var path = Path.GetDirectoryName(fileInfo.FullName)!;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);

        if (singleFile) {
            using var fileStreamCff = new FileStream(Path.Combine(path, fileNameWithoutExtension) + GlobalSettings.ExtensionCFF, FileMode.Create);
            SaveToStreamCFGSection(fileStreamCff, singleFile, dataFileType);
            SaveToStreamDATSection(fileStreamCff, singleFile, dataFileType);
        }
        else {
            using var fileStreamCfg = new FileStream(Path.Combine(path, fileNameWithoutExtension) + GlobalSettings.ExtensionCFG, FileMode.Create);
            SaveToStreamCFGSection(fileStreamCfg, singleFile, dataFileType);

            using var fileStreamDat = new FileStream(Path.Combine(path, fileNameWithoutExtension) + GlobalSettings.ExtensionDAT, FileMode.Create);
            SaveToStreamDATSection(fileStreamDat, singleFile, dataFileType);
        }
    }

    private void SaveToStreamCFGSection(Stream stream, bool singleFile, DataFileType dataFileType)
    {
        CalculateScaleFactorAB(dataFileType);

        var strings = new List<string>();

        if (singleFile) {
            strings.Add("--- file type: CFG ---");
        }

        strings.Add(_stationName + GlobalSettings.Comma +
                    _deviceId + GlobalSettings.Comma +
                    "2013");

        strings.Add((_analogChannels.Count + _digitalChannels.Count).ToString() + GlobalSettings.Comma +
                    _analogChannels.Count + "A" + GlobalSettings.Comma +
                    _digitalChannels.Count + "D");

        foreach (var analogChannel in _analogChannels) {
            strings.Add(analogChannel.ToCFGString());
        }

        for (var i = 0; i < _digitalChannels.Count; i++) {
            strings.Add(_digitalChannels[i].ToCFGString());
        }

        strings.Add(_frequency.ToString(CultureInfo.InvariantCulture));

        if (_sampleRates == null || _sampleRates.Count == 0) {
            strings.Add("0");

            strings.Add("0" + GlobalSettings.Comma +
                        _samples.Count);
        }
        else {
            strings.Add(_sampleRates.Count.ToString());

            foreach (var sampleRate in _sampleRates) {
                strings.Add(sampleRate.SamplingFrequency.ToString() + GlobalSettings.Comma +
                            sampleRate.LastSampleNumber);
            }
        }

        strings.Add(StartTime.ToString(GlobalSettings.DateTimeFormatForWrite,
                                       CultureInfo.InvariantCulture));

        strings.Add(TriggerTime.ToString(GlobalSettings.DateTimeFormatForWrite,
                                         CultureInfo.InvariantCulture));

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
                throw new InvalidOperationException("Undefined data file type =" + dataFileType);
        }

        strings.Add("1.0");

        if (singleFile) {
            strings.Add("--- file type: INF ---");
            strings.Add(GlobalSettings.NewLineWindows);
            strings.Add("--- file type: HDR ---");
            strings.Add(GlobalSettings.NewLineWindows);
        }

        foreach (var str in strings.SkipLast(1)) {
            stream.Write(Encoding.UTF8.GetBytes(str));

            if (str != GlobalSettings.NewLineWindows) {
                stream.Write(Encoding.UTF8.GetBytes(GlobalSettings.NewLineWindows));
            }
        }

        stream.Write(Encoding.UTF8.GetBytes(strings[^1]));
    }

    public void SaveToStreamDATSection(Stream stream, bool singleFile, DataFileType dataFileType)
    {
        var strings = new List<string>();

        if (singleFile) {
            if (dataFileType == DataFileType.ASCII) {
                strings.Add("--- file type: DAT ASCII ---");

                foreach (var sample in _samples) {
                    strings.Add(sample.ToASCIIDAT());
                }

                foreach (var str in strings.SkipLast(1)) {
                    stream.Write(Encoding.UTF8.GetBytes(str));
                    stream.Write(Encoding.UTF8.GetBytes(GlobalSettings.NewLineWindows));
                }

                stream.Write(Encoding.UTF8.GetBytes(strings[^1]));
            }
            else {
                //binary
                var byteInOneSample = DataFileHandler.GetByteCountInOneSample(_analogChannels.Count, _digitalChannels.Count, dataFileType);
                strings.Add($"--- file type: DAT BINARY: {byteInOneSample * _samples.Count} ---");

                foreach (var str in strings) {
                    stream.Write(Encoding.UTF8.GetBytes(str));
                    stream.Write(Encoding.UTF8.GetBytes(GlobalSettings.NewLineWindows));
                }

                foreach (var sample in _samples) {
                    stream.Write(sample.ToByteDAT(dataFileType, _analogChannels));
                }
            }
        }
        else {
            if (dataFileType == DataFileType.ASCII) {
                foreach (var sample in _samples) {
                    strings.Add(sample.ToASCIIDAT());
                }

                foreach (var str in strings.SkipLast(1)) {
                    stream.Write(Encoding.UTF8.GetBytes(str));
                    stream.Write(Encoding.UTF8.GetBytes(GlobalSettings.NewLineWindows));
                }

                stream.Write(Encoding.UTF8.GetBytes(strings[^1]));
            }
            else {
                //binary
                foreach (var sample in _samples) {
                    stream.Write(sample.ToByteDAT(dataFileType, _analogChannels));
                }
            }
        }
    }

    private void CalculateScaleFactorAB(DataFileType dataFileType)
    {
        if (dataFileType == DataFileType.Binary ||
            dataFileType == DataFileType.Binary32) {
            //i make it same, but in theory, bin32 can be more precise			
            for (var i = 0; i < _analogChannels.Count; i++) {
                var min = _samples.Min(x => x.AnalogValues[i]);
                var max = _samples.Max(x => x.AnalogValues[i]);
                _analogChannels[i].MultiplierB = (max + min) / 2.0;

                if (max != min) {
                    _analogChannels[i].MultiplierA = (max - min) / 32767.0; //65536						
                }

                _analogChannels[i].Min = -32767; //by standart 1999
                _analogChannels[i].Max = 32767; //by standart 1999					
            }
        }
        else if (dataFileType == DataFileType.ASCII) {
            foreach (var analogChannelInformation in _analogChannels) {
                analogChannelInformation.Min = -32767; //by standart 1999
                analogChannelInformation.Max = 32767; //by standart 1999
            }
        }
    }
}
