using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Wisp.Comtrade
{
	[TestClass]
	public class DataFileSampleTest
	{
		[TestMethod]
		public void ASCIIReadingTest()
		{
			const string str="5 ,667 , -760, 1274,72,, 3.4028235e38,-3.4028235e38,0 ,0,0 ,0,1,1";
			var sample=new DataFileSample(str,6,6);

			Assert.AreEqual(5,				sample.Number);
			Assert.AreEqual(667,			sample.Timestamp);
			Assert.AreEqual(6,				sample.AnalogValues.Length);
			Assert.AreEqual(-760,			sample.AnalogValues[0]);
			Assert.AreEqual(1274,			sample.AnalogValues[1]);
			Assert.AreEqual(72,				sample.AnalogValues[2]);
			Assert.AreEqual(0,				sample.AnalogValues[3]);
			Assert.AreEqual(3.4028235e38,	sample.AnalogValues[4]);
			Assert.AreEqual(-3.4028235e38,	sample.AnalogValues[5]);
			Assert.AreEqual(6,				sample.DigitalValues.Length);
			Assert.AreEqual(false,			sample.DigitalValues[0]);
			Assert.AreEqual(false,			sample.DigitalValues[1]);
			Assert.AreEqual(false,			sample.DigitalValues[2]);
			Assert.AreEqual(false,			sample.DigitalValues[3]);
			Assert.AreEqual(true,			sample.DigitalValues[4]);
			Assert.AreEqual(true,			sample.DigitalValues[5]);
		}
		
		[TestMethod]
		public void CommonBinaryReadingTest()
		{
			byte[] bytes={
				0x05,0x00,0x50,0x00,
				0x9B,0x02,0x00,0x00,
				0x08,0xFD,
				0xFA,0x04,
				0x48,0x00,
				0x3D,0x00,
				0x74,0xFF,
				0x0A,0xFE,
				0x30,0x00
			};
			
			var sample=new DataFileSample(bytes,DataFileType.Binary,6,6);

			Assert.AreEqual(5242885, sample.Number);
			Assert.AreEqual(667, sample.Timestamp);
			Assert.AreEqual(6, sample.AnalogValues.Length);
			Assert.AreEqual(-760, sample.AnalogValues[0]);
			Assert.AreEqual(1274, sample.AnalogValues[1]);
			Assert.AreEqual(72, sample.AnalogValues[2]);
			Assert.AreEqual(61, sample.AnalogValues[3]);
			Assert.AreEqual(-140, sample.AnalogValues[4]);
			Assert.AreEqual(-502, sample.AnalogValues[5]);
			Assert.AreEqual(6, sample.DigitalValues.Length);
			Assert.AreEqual(false, sample.DigitalValues[0]);
			Assert.AreEqual(false, sample.DigitalValues[1]);
			Assert.AreEqual(false, sample.DigitalValues[2]);
			Assert.AreEqual(false, sample.DigitalValues[3]);
			Assert.AreEqual(true, sample.DigitalValues[4]);
			Assert.AreEqual(true, sample.DigitalValues[5]);
		}
		
		[TestMethod]
		public void CommonBinaryWritingTest()
		{
			byte[] bytes={
				0x05,0x00,0x50,0x00,
				0x9B,0x02,0x00,0x00,
				0x08,0xFD,
				0xFA,0x04,
				0x48,0x00,
				0x3D,0x00,
				0x74,0xFF,
				0x0A,0xFE,
				0x30,0x00
				
			};
			
			var sample=new DataFileSample("5242885,667,-760,1274,72,61,-140,-502,0,0,0,0,1,1",6,6);

            var analogInformations = new List<AnalogChannelInformation> {
                new AnalogChannelInformation(string.Empty, string.Empty),
                new AnalogChannelInformation(string.Empty, string.Empty),
                new AnalogChannelInformation(string.Empty, string.Empty),
                new AnalogChannelInformation(string.Empty, string.Empty),
                new AnalogChannelInformation(string.Empty, string.Empty),
                new AnalogChannelInformation(string.Empty, string.Empty)
            };

            var result=sample.ToByteDAT(DataFileType.Binary,analogInformations);
			
			for(int i=0;i<bytes.Length;i++){
				Assert.AreEqual(bytes[i],result[i]);
			}
						
		}
		
		[TestMethod]
		public void DigitalOnlyBinaryReadingTest()
		{
			byte[] bytes={
				0x05,0x00,0x00,0x00,
				0x9B,0x02,0x00,0x00,
				0x0F,0x0F,
				0x5A,0x5A,
				0x01,0x00
			};
			
			var sample=new DataFileSample(bytes,DataFileType.Binary,0,33);

			Assert.AreEqual(5, sample.Number);
			Assert.AreEqual(667, sample.Timestamp);
			Assert.AreEqual(0, sample.AnalogValues.Length);
			Assert.AreEqual(33, sample.DigitalValues.Length);

			Assert.AreEqual(true, sample.DigitalValues[0]);
			Assert.AreEqual(true, sample.DigitalValues[1]);
			Assert.AreEqual(true, sample.DigitalValues[2]);
			Assert.AreEqual(true, sample.DigitalValues[3]);
			Assert.AreEqual(false, sample.DigitalValues[4]);
			Assert.AreEqual(false, sample.DigitalValues[5]);
			Assert.AreEqual(false, sample.DigitalValues[6]);
			Assert.AreEqual(false, sample.DigitalValues[7]);

			Assert.AreEqual(true, sample.DigitalValues[8]);
			Assert.AreEqual(true, sample.DigitalValues[9]);
			Assert.AreEqual(true, sample.DigitalValues[10]);
			Assert.AreEqual(true, sample.DigitalValues[11]);
			Assert.AreEqual(false, sample.DigitalValues[12]);
			Assert.AreEqual(false, sample.DigitalValues[13]);
			Assert.AreEqual(false, sample.DigitalValues[14]);
			Assert.AreEqual(false, sample.DigitalValues[15]);

			Assert.AreEqual(false, sample.DigitalValues[16]);
			Assert.AreEqual(true, sample.DigitalValues[17]);
			Assert.AreEqual(false, sample.DigitalValues[18]);
			Assert.AreEqual(true, sample.DigitalValues[19]);
			Assert.AreEqual(true, sample.DigitalValues[20]);
			Assert.AreEqual(false, sample.DigitalValues[21]);
			Assert.AreEqual(true, sample.DigitalValues[22]);
			Assert.AreEqual(false, sample.DigitalValues[23]);

			Assert.AreEqual(false, sample.DigitalValues[24]);
			Assert.AreEqual(true, sample.DigitalValues[25]);
			Assert.AreEqual(false, sample.DigitalValues[26]);
			Assert.AreEqual(true, sample.DigitalValues[27]);
			Assert.AreEqual(true, sample.DigitalValues[28]);
			Assert.AreEqual(false, sample.DigitalValues[29]);
			Assert.AreEqual(true, sample.DigitalValues[30]);
			Assert.AreEqual(false, sample.DigitalValues[31]);

			Assert.AreEqual(true, sample.DigitalValues[32]);
		}
	}
}

