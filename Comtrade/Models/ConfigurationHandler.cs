using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Wisp.Comtrade.Utils;

namespace Wisp.Comtrade.Models;

public class ConfigurationHandler
{
    private List<AnalogChannel> _analogChannelInformationList = [];
    private List<DigitalChannel> _digitalChannelInformationList = [];
    private List<SampleRate> _sampleRates = [];
    public DataFileType DataFileType = DataFileType.Undefined;
    internal double TimeMultiplicationFactor = 1.0;

    public ConfigurationHandler()
    {
    }

    public ConfigurationHandler(string fullPathToFileCFG)
    {
        Parse(File.ReadAllLines(fullPathToFileCFG, Encoding.Default));
    }

    internal ConfigurationHandler(string[] lines)
    {
        Parse(lines);
    }

    public string StationName { get; private set; } = string.Empty;
    public string DeviceId { get; private set; } = string.Empty;
    public ComtradeVersion Version { get; private set; } = ComtradeVersion.V1991;
    public int AnalogChannelsCount { get; private set; }
    public int DigitalChannelsCount { get; private set; }
    public IReadOnlyList<SampleRate> SampleRates => _sampleRates;
    public IReadOnlyList<AnalogChannel> AnalogChannelInformationList => _analogChannelInformationList;
    public IReadOnlyList<DigitalChannel> DigitalChannelInformationList => _digitalChannelInformationList;
    public double Frequency { get; set; } = 50.0;
    public int SamplingRateCount { get; set; }

    /// <summary>
    ///     Time of first value in data
    ///     Max time resolution 100ns (.net DateTime constrain)
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    ///     Time of trigger point
    ///     Max time resolution 100ns (.net DateTime constrain)
    /// </summary>
    public DateTime TriggerTime { get; private set; }
    internal bool TimeLineNanoSecondResolution { get; set; }

    private void Parse(string[] strings)
    {
        ParseFirstLine(strings[0]);
        ParseSecondLine(strings[1]);

        _analogChannelInformationList = new List<AnalogChannel>();

        for (var i = 0; i < AnalogChannelsCount; i++)
        {
            _analogChannelInformationList.Add(new AnalogChannel(strings[2 + i]));
        }

        _digitalChannelInformationList = new List<DigitalChannel>();

        for (var i = 0; i < DigitalChannelsCount; i++)
        {
            _digitalChannelInformationList.Add(new DigitalChannel(strings[2 + i + AnalogChannelsCount]));
        }

        var strIndex = 2 + AnalogChannelsCount + DigitalChannelsCount;
        ParseFrequencyLine(strings[strIndex++]);

        ParseNumberOfSampleRates(strings[strIndex++]);

        _sampleRates = new List<SampleRate>();

        if (SamplingRateCount == 0)
        {
            _sampleRates.Add(new SampleRate(strings[strIndex++]));
        }
        else
        {
            for (var i = 0; i < SamplingRateCount; i++)
            {
                _sampleRates.Add(new SampleRate(strings[strIndex + i]));
            }

            strIndex += SamplingRateCount;
        }

        StartTime = ParseDateTime(strings[strIndex++], out var nanosecond);
        TimeLineNanoSecondResolution = nanosecond;
        TriggerTime = ParseDateTime(strings[strIndex++], out _);

        ParseDataFileType(strings[strIndex++]);

        ParseTimeMultiplicationFactor(strings[strIndex++]);

        //TODO add non-essential fields for 2013 standard
    }

    private void ParseFirstLine(string firstLine)
    {
        firstLine = firstLine.Replace(GlobalSettings.WhiteSpace.ToString(), string.Empty);
        var values = firstLine.Split(GlobalSettings.Comma);
        StationName = values[0];
        DeviceId = values[1];

        if (values.Length == 3)
        {
            Version = ComtradeVersionConverter.Get(values[2]);
        }
    }

    private void ParseSecondLine(string secondLine)
    {
        secondLine = secondLine.Replace(GlobalSettings.WhiteSpace.ToString(), string.Empty);
        var values = secondLine.Split(GlobalSettings.Comma);
        //values[0];// not used, equal to the sum of the next two
        AnalogChannelsCount = Convert.ToInt32(values[1].TrimEnd('A'), CultureInfo.InvariantCulture);
        DigitalChannelsCount = Convert.ToInt32(values[2].TrimEnd('D'), CultureInfo.InvariantCulture);
    }

    private void ParseFrequencyLine(string frequenceLine)
    {
        Frequency = Convert.ToDouble(frequenceLine.Trim(), CultureInfo.InvariantCulture);
    }

    private void ParseNumberOfSampleRates(string str)
    {
        SamplingRateCount = Convert.ToInt32(str.Trim(), CultureInfo.InvariantCulture);
    }

    public static DateTime ParseDateTime(string str, out bool nanoSecond)
    {
        if (DateTime.TryParseExact(str, GlobalSettings.DateTimeFormatForParseMicroSecond,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.AllowWhiteSpaces,
                                   out var result))
        {
            nanoSecond = false;
            return result;
        }

        var strings = str.Split('.');
        str = strings[0] + '.' + strings[1].Substring(0, Math.Min(7, strings[1].Length));

        DateTime.TryParseExact(str, GlobalSettings.DateTimeFormatForParseNanoSecond,
                               CultureInfo.InvariantCulture,
                               DateTimeStyles.AllowWhiteSpaces,
                               out result);

        nanoSecond = true;
        return result;
    }

    private void ParseDataFileType(string str)
    {
        DataFileType = DataFileTypeConverter.Get(str.Trim());
    }

    private void ParseTimeMultiplicationFactor(string str)
    {
        TimeMultiplicationFactor = Convert.ToDouble(str.Trim(), CultureInfo.InvariantCulture);
    }
}
