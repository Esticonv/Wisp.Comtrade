using System;
using System.Collections.Generic;
using System.Linq;
namespace Wisp.Comtrade.Models;

public class DataFileHandler
{
    private const int SampleNumberLength = 4;
    private const int TimeStampLength = 4;
    private const int Digital16ChannelLength = 2;
    internal readonly DataFileSample[] Samples;

    internal DataFileHandler(string[] strings, ConfigurationHandler configuration)
    {
        var samplesCount = configuration.SampleRates[^1].LastSampleNumber;
        Samples = new DataFileSample[samplesCount];

        if (configuration.DataFileType == DataFileType.ASCII)
        {
            strings = strings.Where(x => x != string.Empty).ToArray(); //removing empty strings (when *.dat file not following Standard)

            for (var i = 0; i < samplesCount; i++)
            {
                Samples[i] = new DataFileSample(strings[i], configuration.AnalogChannelsCount, configuration.DigitalChannelsCount);
            }
        }
        else
        {
            throw new InvalidOperationException($"Configuration dataFileType must be ASCII, but was {configuration.DataFileType}");
        }
    }

    internal DataFileHandler(IReadOnlyList<byte> bytes, ConfigurationHandler configuration)
    {
        var samplesCount = configuration.SampleRates[^1].LastSampleNumber;
        Samples = new DataFileSample[samplesCount];

        if (configuration.DataFileType is DataFileType.Binary or DataFileType.Binary32 or DataFileType.Float32)
        {
            var oneSampleLength = GetByteCountInOneSample(configuration.AnalogChannelsCount, configuration.DigitalChannelsCount, configuration.DataFileType);

            for (var i = 0; i < samplesCount; i++)
            {
                var bytesOneSample = new byte[oneSampleLength];

                for (var j = 0; j < oneSampleLength; j++)
                {
                    bytesOneSample[j] = bytes[i * oneSampleLength + j];
                }

                Samples[i] = new DataFileSample(bytesOneSample, configuration.DataFileType, configuration.AnalogChannelsCount, configuration.DigitalChannelsCount);
            }
        }
        else
        {
            throw new InvalidOperationException($"Configuration dataFileType must be Binary, Binary32 or Float , but was {configuration.DataFileType}");
        }
    }

    public static int GetDigitalByteCount(int digitalChannelsCount)
    {
        return (digitalChannelsCount / 16 + (digitalChannelsCount % 16 == 0 ? 0 : 1)) * Digital16ChannelLength;
    }

    public static int GetByteCountInOneSample(int analogsChannelsCount, int digitalChannelsCount, DataFileType dataFileType)
    {
        var analogOneChannelLength = dataFileType == DataFileType.Binary ? 2 : 4;

        return SampleNumberLength + TimeStampLength + analogsChannelsCount * analogOneChannelLength + GetDigitalByteCount(digitalChannelsCount);
    }
}
