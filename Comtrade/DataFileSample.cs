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
            this.number = number;
            this.timestamp = timestamp;
            this.analogs = analogs;
            this.digitals = digitals;
        }

        public DataFileSample(string asciiLine, int analogCount, int digitalCount)
        {
            asciiLine = asciiLine.Replace(GlobalSettings.whiteSpace.ToString(), string.Empty);
            var strings = asciiLine.Split(GlobalSettings.commaDelimiter);

            analogs = new double[analogCount];
            digitals = new bool[digitalCount];

            number = Convert.ToInt32(strings[0]);
            timestamp = Convert.ToInt32(strings[1]);

            for (int i = 0; i < analogCount; i++)
            {
                if (strings[i + 2] != string.Empty)
                {//by Standart, can be missing value. In that case by default=0
                    analogs[i] = Convert.ToDouble(strings[i + 2], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            for (int i = 0; i < digitalCount; i++)
            {
                digitals[i] = Convert.ToBoolean(Convert.ToInt32(strings[i + 2 + analogCount]));
            }
        }

        public DataFileSample(byte[] bytes, DataFileType dataFileType, int analogCount, int digitalCount)
        {
            analogs = new double[analogCount];
            digitals = new bool[digitalCount];

            number = BitConverter.ToInt32(bytes, 0);
            timestamp = BitConverter.ToInt32(bytes, 4);

            int digitalByteStart;
            if (dataFileType == DataFileType.Binary)
            {
                for (int i = 0; i < analogCount; i++)
                {
                    analogs[i] = BitConverter.ToInt16(bytes, 8 + i * 2);
                }
                digitalByteStart = 8 + 2 * analogCount;
            }
            else
            {
                if (dataFileType == DataFileType.Binary32)
                {
                    for (int i = 0; i < analogCount; i++)
                    {//TODO add test				
                        analogs[i] = BitConverter.ToInt32(bytes, 8 + i * 4);
                    }
                }
                else if (dataFileType == DataFileType.Float32)
                {
                    for (int i = 0; i < analogCount; i++)
                    {//TODO add test			
                        analogs[i] = BitConverter.ToSingle(bytes, 8 + i * 4);
                    }
                }

                digitalByteStart = 8 + 4 * analogCount;
            }

            //int digitalByteCount=DataFileHandler.GetDigitalByteCount(digitalCount);
            for (int i = 0; i < digitalCount; i++)
            {
                int digitalByteIterator = i / 8;
                digitals[i] = Convert.ToBoolean(bytes[digitalByteStart + digitalByteIterator] >> i - digitalByteIterator * 8 & 1);
            }
        }

        public string ToASCIIDAT()
        {
            string result = string.Empty;
            result += number.ToString();
            result += GlobalSettings.commaDelimiter +
                timestamp.ToString();
            foreach (var analog in analogs)
            {
                result += GlobalSettings.commaDelimiter +
                    analog.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            foreach (var digital in digitals)
            {
                result += GlobalSettings.commaDelimiter +
                    Convert.ToInt32(digital).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return result;
        }


        public byte[] ToByteDAT(DataFileType dataFileType, IReadOnlyList<AnalogChannelInformation> analogInformations)
        {
            var result = new byte[DataFileHandler.GetByteCountInOneSample(analogs.Length, digitals.Length, dataFileType)];
            int analogOneChannelLength = dataFileType == DataFileType.Binary ? 2 : 4;
            int digitalByteStart = 8 + analogOneChannelLength * analogs.Length;

            BitConverter.GetBytes(number).CopyTo(result, 0);
            BitConverter.GetBytes(timestamp).CopyTo(result, 4);

            switch (dataFileType)
            {
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

        void AnalogsToBinaryDAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
        {
            for (int i = 0; i < analogs.Length; i++)
            {
                short s = (short)((analogs[i] - analogInformations[i].b) / analogInformations[i].a);
                BitConverter.GetBytes(s).CopyTo(result, 8 + i * 2);
            }
        }

        void AnalogsToBinary32DAT(byte[] result, IReadOnlyList<AnalogChannelInformation> analogInformations)
        {
            for (int i = 0; i < analogs.Length; i++)
            {
                int s = (int)((analogs[i] - analogInformations[i].b) / analogInformations[i].a);
                BitConverter.GetBytes(s).CopyTo(result, 8 + i * 4);
            }
        }

        void AnalogsToFloat32DAT(byte[] result)
        {
            for (int i = 0; i < analogs.Length; i++)
            {
                BitConverter.GetBytes((float)analogs[i]).CopyTo(result, 8 + i * 4);
            }
        }

        void DigitalsToDAT(byte[] result, int digitalByteStart)
        {
            int byteIndex = 0;
            byte s = 0;
            for (int i = 0; i < digitals.Length; i++)
            {
                s = (byte)(Convert.ToInt32(s) | Convert.ToInt32(digitals[i]) << i - byteIndex * 8);

                if ((i + 1) % 8 == 0 || i + 1 == digitals.Length)
                {
                    result[digitalByteStart + byteIndex] = s;
                    s = 0;
                    byteIndex++;
                }
            }
        }
    }
}
