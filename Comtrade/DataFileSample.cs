using System;
using System.Collections.Generic;
using System.Globalization;

namespace Wisp.Comtrade;

public class DataFileSample
{
    public DataFileSample(int number, int timestamp, double[] analogValues, bool[] digitalValues)
    {
        Number = number;
        Timestamp = timestamp;
        AnalogValues = analogValues;
        DigitalValues = digitalValues;
    }

    public DataFileSample(string asciiLine, int analogCount, int digitalCount)
    {
        asciiLine = asciiLine.Replace(GlobalSettings.WhiteSpace.ToString(), string.Empty);
        var strings = asciiLine.Split(GlobalSettings.Comma);

        AnalogValues = new double[analogCount];
        DigitalValues = new bool[digitalCount];

        Number = Convert.ToInt32(strings[0]);
        Timestamp = Convert.ToInt32(strings[1]);

        for (var i = 0; i < analogCount; i++) {
            if (strings[i + 2] != string.Empty) {
                //by Standard, can be missing value. In that case by default=0
                AnalogValues[i] = Convert.ToDouble(strings[i + 2], CultureInfo.InvariantCulture);
            }
        }

        for (var i = 0; i < digitalCount; i++) {
            DigitalValues[i] = Convert.ToBoolean(Convert.ToInt32(strings[i + 2 + analogCount]));
        }
    }

    public DataFileSample(byte[] bytes, DataFileType dataFileType, int analogCount, int digitalCount)
    {
        AnalogValues = new double[analogCount];
        DigitalValues = new bool[digitalCount];

        Number = BitConverter.ToInt32(bytes, 0);
        Timestamp = BitConverter.ToInt32(bytes, 4);

        int digitalByteStart;

        if (dataFileType == DataFileType.Binary) {
            for (var i = 0; i < analogCount; i++) {
                AnalogValues[i] = BitConverter.ToInt16(bytes, 8 + i * 2);
            }

            digitalByteStart = 8 + 2 * analogCount;
        }
        else {
            if (dataFileType == DataFileType.Binary32) {
                for (var i = 0; i < analogCount; i++)
                    //TODO add test
                {
                    AnalogValues[i] = BitConverter.ToInt32(bytes, 8 + i * 4);
                }
            }
            else if (dataFileType == DataFileType.Float32) {
                for (var i = 0; i < analogCount; i++)
                    //TODO add test
                {
                    AnalogValues[i] = BitConverter.ToSingle(bytes, 8 + i * 4);
                }
            }

            digitalByteStart = 8 + 4 * analogCount;
        }

        //var digitalByteCount = DataFileHandler.GetDigitalByteCount(digitalCount);

        for (var i = 0; i < digitalCount; i++) {
            var digitalByteIterator = i / 8;

            DigitalValues[i] = Convert.ToBoolean((bytes[digitalByteStart + digitalByteIterator] >> (i - digitalByteIterator * 8)) & 1);
        }
    }

    public int Number { get; }

    /// <summary>
    ///     microsecond or nanosecond defined by CFG (according STD)
    ///     Note: I do not know, where in CFG it is defined, suppose always be microsecond
    /// </summary>
    public int Timestamp { get; }

    public double[] AnalogValues { get; }
    public bool[] DigitalValues { get; }

    public string ToASCIIDAT()
    {
        var result = string.Empty;
        result += Number.ToString();

        result += GlobalSettings.Comma + Timestamp.ToString();

        foreach (var analog in AnalogValues) {
            result += GlobalSettings.Comma + analog.ToString(CultureInfo.InvariantCulture);
        }

        foreach (var digital in DigitalValues) {
            result += GlobalSettings.Comma + Convert.ToInt32(digital).ToString(CultureInfo.InvariantCulture);
        }

        return result;
    }

    public byte[] ToByteDAT(DataFileType dataFileType, IReadOnlyList<AnalogChannelInformation> analogInformations)
    {
        var result = new byte[DataFileHandler.GetByteCountInOneSample(AnalogValues.Length, DigitalValues.Length, dataFileType)];

        var analogOneChannelLength = dataFileType == DataFileType.Binary ? 2 : 4;
        var digitalByteStart = 8 + analogOneChannelLength * AnalogValues.Length;

        BitConverter.GetBytes(Number).CopyTo(result, 0);
        BitConverter.GetBytes(Timestamp).CopyTo(result, 4);

        switch (dataFileType) {
            case DataFileType.Binary:
                AnalogsToBinaryDAT(result, analogInformations);
                break;
            case DataFileType.Binary32:
                AnalogsToBinary32DAT(result, analogInformations);
                break;
            case DataFileType.Float32:
                AnalogsToFloat32DAT(result);
                break;
            default:
                throw new InvalidOperationException("Not supported file type DAT");
        }

        DigitalsToDAT(result, digitalByteStart);
        return result;
    }

    private void AnalogsToBinaryDAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
    {
        for (var i = 0; i < AnalogValues.Length; i++) {
            var s = (short) ((AnalogValues[i] - analogInformations[i].MultiplierB) / analogInformations[i].MultiplierA);

            BitConverter.GetBytes(s).CopyTo(result, 8 + i * 2);
        }
    }

    private void AnalogsToBinary32DAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
    {
        for (var i = 0; i < AnalogValues.Length; i++) {
            var s = (int) ((AnalogValues[i] - analogInformations[i].MultiplierB) / analogInformations[i].MultiplierA);

            BitConverter.GetBytes(s).CopyTo(result, 8 + i * 4);
        }
    }

    private void AnalogsToFloat32DAT(byte[] result)
    {
        for (var i = 0; i < AnalogValues.Length; i++) {
            BitConverter.GetBytes((float) AnalogValues[i]).CopyTo(result, 8 + i * 4);
        }
    }

    private void DigitalsToDAT(byte[] result, int digitalByteStart)
    {
        var byteIndex = 0;
        byte s = 0;

        for (var i = 0; i < DigitalValues.Length; i++) {
            s = (byte) (Convert.ToInt32(s) | (Convert.ToInt32(DigitalValues[i]) << (i - byteIndex * 8)));

            if ((i + 1) % 8 == 0 || i + 1 == DigitalValues.Length) {
                result[digitalByteStart + byteIndex] = s;
                s = 0;
                byteIndex++;
            }
        }
    }
}
