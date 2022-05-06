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

			Assert.AreEqual(5,				sample.number);
			Assert.AreEqual(667,			sample.timestamp);
			Assert.AreEqual(6,				sample.analogs.Length);
			Assert.AreEqual(-760,			sample.analogs[0]);
			Assert.AreEqual(1274,			sample.analogs[1]);
			Assert.AreEqual(72,				sample.analogs[2]);
			Assert.AreEqual(0,				sample.analogs[3]);
			Assert.AreEqual(3.4028235e38,	sample.analogs[4]);
			Assert.AreEqual(-3.4028235e38,	sample.analogs[5]);
			Assert.AreEqual(6,				sample.digitals.Length);
			Assert.AreEqual(false,			sample.digitals[0]);
			Assert.AreEqual(false,			sample.digitals[1]);
			Assert.AreEqual(false,			sample.digitals[2]);
			Assert.AreEqual(false,			sample.digitals[3]);
			Assert.AreEqual(true,			sample.digitals[4]);
			Assert.AreEqual(true,			sample.digitals[5]);
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

			Assert.AreEqual(5242885, sample.number);
			Assert.AreEqual(667, sample.timestamp);
			Assert.AreEqual(6, sample.analogs.Length);
			Assert.AreEqual(-760, sample.analogs[0]);
			Assert.AreEqual(1274, sample.analogs[1]);
			Assert.AreEqual(72, sample.analogs[2]);
			Assert.AreEqual(61, sample.analogs[3]);
			Assert.AreEqual(-140, sample.analogs[4]);
			Assert.AreEqual(-502, sample.analogs[5]);
			Assert.AreEqual(6, sample.digitals.Length);
			Assert.AreEqual(false, sample.digitals[0]);
			Assert.AreEqual(false, sample.digitals[1]);
			Assert.AreEqual(false, sample.digitals[2]);
			Assert.AreEqual(false, sample.digitals[3]);
			Assert.AreEqual(true, sample.digitals[4]);
			Assert.AreEqual(true, sample.digitals[5]);

			/*Assert.That(sample.number,Is.EqualTo(5242885));
			Assert.That(sample.timestamp, Is.EqualTo(667));
			Assert.That(sample.analogs.Length,Is.EqualTo(6));
			Assert.That(sample.analogs[0],Is.EqualTo(-760));
			Assert.That(sample.analogs[1],Is.EqualTo(1274));
			Assert.That(sample.analogs[2],Is.EqualTo(72));
			Assert.That(sample.analogs[3],Is.EqualTo(61));
			Assert.That(sample.analogs[4],Is.EqualTo(-140));
			Assert.That(sample.analogs[5],Is.EqualTo(-502));
			Assert.That(sample.digitals.Length,Is.EqualTo(6));
			Assert.That(sample.digitals[0],Is.EqualTo(false));
			Assert.That(sample.digitals[1],Is.EqualTo(false));
			Assert.That(sample.digitals[2],Is.EqualTo(false));
			Assert.That(sample.digitals[3],Is.EqualTo(false));
			Assert.That(sample.digitals[4],Is.EqualTo(true));
			Assert.That(sample.digitals[5],Is.EqualTo(true));*/
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
				//Assert.That(result[i],Is.EqualTo(bytes[i]));
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

			Assert.AreEqual(5, sample.number);
			Assert.AreEqual(667, sample.timestamp);
			Assert.AreEqual(0, sample.analogs.Length);
			Assert.AreEqual(33, sample.digitals.Length);

			Assert.AreEqual(true, sample.digitals[0]);
			Assert.AreEqual(true, sample.digitals[1]);
			Assert.AreEqual(true, sample.digitals[2]);
			Assert.AreEqual(true, sample.digitals[3]);
			Assert.AreEqual(false, sample.digitals[4]);
			Assert.AreEqual(false, sample.digitals[5]);
			Assert.AreEqual(false, sample.digitals[6]);
			Assert.AreEqual(false, sample.digitals[7]);

			Assert.AreEqual(true, sample.digitals[8]);
			Assert.AreEqual(true, sample.digitals[9]);
			Assert.AreEqual(true, sample.digitals[10]);
			Assert.AreEqual(true, sample.digitals[11]);
			Assert.AreEqual(false, sample.digitals[12]);
			Assert.AreEqual(false, sample.digitals[13]);
			Assert.AreEqual(false, sample.digitals[14]);
			Assert.AreEqual(false, sample.digitals[15]);

			Assert.AreEqual(false, sample.digitals[16]);
			Assert.AreEqual(true, sample.digitals[17]);
			Assert.AreEqual(false, sample.digitals[18]);
			Assert.AreEqual(true, sample.digitals[19]);
			Assert.AreEqual(true, sample.digitals[20]);
			Assert.AreEqual(false, sample.digitals[21]);
			Assert.AreEqual(true, sample.digitals[22]);
			Assert.AreEqual(false, sample.digitals[23]);

			Assert.AreEqual(false, sample.digitals[24]);
			Assert.AreEqual(true, sample.digitals[25]);
			Assert.AreEqual(false, sample.digitals[26]);
			Assert.AreEqual(true, sample.digitals[27]);
			Assert.AreEqual(true, sample.digitals[28]);
			Assert.AreEqual(false, sample.digitals[29]);
			Assert.AreEqual(true, sample.digitals[30]);
			Assert.AreEqual(false, sample.digitals[31]);

			Assert.AreEqual(true, sample.digitals[32]);

			/*Assert.That(sample.number,Is.EqualTo(5));
			Assert.That(sample.timestamp, Is.EqualTo(667));
			Assert.That(sample.analogs.Length,Is.EqualTo(0));		
			Assert.That(sample.digitals.Length,Is.EqualTo(33));
			
			Assert.That(sample.digitals[0],Is.EqualTo(true));
			Assert.That(sample.digitals[1],Is.EqualTo(true));
			Assert.That(sample.digitals[2],Is.EqualTo(true));
			Assert.That(sample.digitals[3],Is.EqualTo(true));
			Assert.That(sample.digitals[4],Is.EqualTo(false));
			Assert.That(sample.digitals[5],Is.EqualTo(false));
			Assert.That(sample.digitals[6],Is.EqualTo(false));
			Assert.That(sample.digitals[7],Is.EqualTo(false));
			
			Assert.That(sample.digitals[8],Is.EqualTo(true));
			Assert.That(sample.digitals[9],Is.EqualTo(true));
			Assert.That(sample.digitals[10],Is.EqualTo(true));
			Assert.That(sample.digitals[11],Is.EqualTo(true));
			Assert.That(sample.digitals[12],Is.EqualTo(false));
			Assert.That(sample.digitals[13],Is.EqualTo(false));
			Assert.That(sample.digitals[14],Is.EqualTo(false));
			Assert.That(sample.digitals[15],Is.EqualTo(false));
			
			Assert.That(sample.digitals[16],Is.EqualTo(false));			
			Assert.That(sample.digitals[17],Is.EqualTo(true));
			Assert.That(sample.digitals[18],Is.EqualTo(false));
			Assert.That(sample.digitals[19],Is.EqualTo(true));
			Assert.That(sample.digitals[20],Is.EqualTo(true));
			Assert.That(sample.digitals[21],Is.EqualTo(false));
			Assert.That(sample.digitals[22],Is.EqualTo(true));
			Assert.That(sample.digitals[23],Is.EqualTo(false));
			
			Assert.That(sample.digitals[24],Is.EqualTo(false));
			Assert.That(sample.digitals[25],Is.EqualTo(true));
			Assert.That(sample.digitals[26],Is.EqualTo(false));
			Assert.That(sample.digitals[27],Is.EqualTo(true));
			Assert.That(sample.digitals[28],Is.EqualTo(true));
			Assert.That(sample.digitals[29],Is.EqualTo(false));
			Assert.That(sample.digitals[30],Is.EqualTo(true));
			Assert.That(sample.digitals[31],Is.EqualTo(false));
			
			Assert.That(sample.digitals[32],Is.EqualTo(true));*/
		}
	}
}

