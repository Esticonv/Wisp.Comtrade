using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Wisp.Comtrade
{
	[TestClass]
	public class RecordWriterTest
	{
		public const string rootYandexDiskDirectory = @"C:\Users\Esti\YandexDisk\";
		const string pathDirectory=		rootYandexDiskDirectory + @"Oscillogram\AutoCreated\";
		const string fullPathAscii=	rootYandexDiskDirectory + @"Oscillogram\AutoCreated\ascii.cfg";
		const string fullPathBinary = rootYandexDiskDirectory + @"Oscillogram\AutoCreated\binary.cfg";
		//const string fullPathAsciiTwo=	rootYandexDiskDirectory + @"Oscillogram\AutoCreated\ascii2.cfg";
		const string fullPathAsciiSingleFile = rootYandexDiskDirectory + @"Oscillogram\AutoCreated\asciiSingleFile.cff";
		const string fullPathBinarySingleFile = rootYandexDiskDirectory + @"Oscillogram\AutoCreated\binarySingleFile.cff";
		//const string fullPathAsciiTwo = rootYandexDiskDirectory + @"Oscillogram\AutoCreated\ascii2.cfg";

		RecordWriter GetWriterToTest()
		{
			var writer=new RecordWriter();
			writer.AddAnalogChannel(new AnalogChannelInformation("channel1a","A"));
			writer.AddAnalogChannel(new AnalogChannelInformation("channel2a","B"));
			writer.AddAnalogChannel(new AnalogChannelInformation("channel3a","C"));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel1b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel2b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel3b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel4b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel5b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel6b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel7b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel8b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel9b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel10b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel11b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel12b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel13b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel14b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel15b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel16b",""));
			writer.AddDigitalChannel(new DigitalChannelInformation("channel17b",""));
			writer.AddSample(0,
			                 new double[]{0,0,0},
			                 new bool[]{
			                 	true,true,true,true,
			                 	true,true,true,true,
			                    true,true,true,true,
			                    true,true,true,true,
			                    true});
			writer.AddSample(500,
			                 new double[]{1.0,2.0,3.0},
			                 new bool[]{
			                 	false,false,false,false,
			                 	false,false,false,false,
			                    false,false,false,false,
			                    false,false,false,false,
			                    false});
			writer.AddSample(1000,
			                 new double[]{-1.0,2.0,-3.5},
			                 new bool[]{
			                 	false,false,false,false,
			                 	true,true,true,true,
			                    false,true,false,true,
			                    true,false,true,false,
			                    true});
			
			writer.AddSample(1500,
			                 new double[]{5.0,5.0,5.0},
			                 new bool[]{
			                 	false,false,false,false,
			                 	true,true,true,true,
			                    false,true,false,true,
			                    true,false,true,false,
			                    true});
			
			writer.StartTime=new DateTime(1234567890);
			writer.TriggerTime=new DateTime(1234569000);
			return writer;
		}
		
		void ReaderAsserts(string fullpath)
		{
			var reader=new RecordReader(fullpath);
			Assert.AreEqual(3, reader.Configuration.AnalogChannelInformationList.Count);
			Assert.AreEqual(17, reader.Configuration.DigitalChannelInformationList.Count);

			var timeLine=reader.GetTimeLine();
			var analogs1=reader.GetAnalogPrimaryChannel(0);
			var analogs2=reader.GetAnalogPrimaryChannel(1);
			var analogs3=reader.GetAnalogPrimaryChannel(2);
			var digitals1=reader.GetDigitalChannel(0);
			var digitals5=reader.GetDigitalChannel(4);
			var digitals17=reader.GetDigitalChannel(16);

			Assert.AreEqual(   0,	timeLine[0], 0.01);
			Assert.AreEqual( 500,	timeLine[1], 0.01);
			Assert.AreEqual(1000,	timeLine[2], 0.01);

			Assert.AreEqual(0,		analogs1[0], 0.01);
			Assert.AreEqual(1.0,	analogs1[1], 0.01);
			Assert.AreEqual(-1.0,	analogs1[2], 0.01);

			Assert.AreEqual(0,		analogs2[0], 0.01);
			Assert.AreEqual(2.0,	analogs2[1], 0.01);
			Assert.AreEqual(2.0,	analogs2[2], 0.01);

			Assert.AreEqual(0,		analogs3[0], 0.01);
			Assert.AreEqual(3.0,	analogs3[1], 0.01);
			Assert.AreEqual(-3.5,	analogs3[2], 0.01);

			Assert.AreEqual(true,	digitals1[0]);
			Assert.AreEqual(false,	digitals1[1]);
			Assert.AreEqual(false,	digitals1[2]);

			Assert.AreEqual(true,	digitals5[0]);
			Assert.AreEqual(false,	digitals5[1]);
			Assert.AreEqual(true,	digitals5[2]);

			Assert.AreEqual(true,	digitals17[0]);
			Assert.AreEqual(false,	digitals17[1]);
			Assert.AreEqual(true,	digitals17[2]);

			Assert.AreEqual(new DateTime(1234567890), reader.Configuration.StartTime);
			Assert.AreEqual(new DateTime(1234569000), reader.Configuration.TriggerTime);
		}
						
		[TestMethod]
		public void SaveToFileTwoFilesAsciiTest()
		{
			var writer=this.GetWriterToTest();
			writer.SaveToFile(fullPathAscii, singleFile:false, DataFileType.ASCII);			
			this.ReaderAsserts(fullPathAscii);			
		}
		
		[TestMethod]
		public void SaveToFileTwoFilesBinaryTest()
		{
			var writer=this.GetWriterToTest();
			writer.SaveToFile(fullPathBinary, singleFile: false, DataFileType.Binary);			
			this.ReaderAsserts(fullPathBinary);			
		}

		[TestMethod]
		public void SaveToFileSingleAsciiTest()
		{
			var writer = this.GetWriterToTest();
			writer.SaveToFile(fullPathAsciiSingleFile, singleFile: true, DataFileType.ASCII);
			this.ReaderAsserts(fullPathAsciiSingleFile);
		}

		[TestMethod]
		public void SaveToFileSingleFileBinaryTest()
		{
			var writer = this.GetWriterToTest();
			writer.SaveToFile(fullPathBinarySingleFile, singleFile: true, DataFileType.Binary);
			this.ReaderAsserts(fullPathBinarySingleFile);
		}

	}
}

