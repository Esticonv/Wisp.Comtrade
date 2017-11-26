/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 07.06.2017
 * Time: 12:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
#if TEST
using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class RecordWriterTest
	{
		const string pathDirectory=   @"D:\YandexDisk\Oscillogram\AutoCreated\";
		const string fullPathAsciiOne=@"D:\YandexDisk\Oscillogram\AutoCreated\ascii.cfg";
		const string fullPathAsciiTwo=@"D:\YandexDisk\Oscillogram\AutoCreated\ascii2.cfg";
		
		
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
			
			writer.startTime=new DateTime(1234567890);
			writer.triggerTime=new DateTime(1234569000);
			return writer;
		}
		
		void ReaderAsserts(string fullpath)
		{
			var reader=new RecordReader(fullpath);
			Assert.That(reader.Configuration.AnalogChannelInformations.Count,Is.EqualTo(3));
			Assert.That(reader.Configuration.DigitalChannelInformations.Count,Is.EqualTo(17));
			
			var timeLine=reader.GetTimeLine();
			var analogs1=reader.GetAnalogPrimaryChannel(0);
			var analogs2=reader.GetAnalogPrimaryChannel(1);
			var analogs3=reader.GetAnalogPrimaryChannel(2);
			var digitals1=reader.GetDigitalChannel(0);
			var digitals5=reader.GetDigitalChannel(4);
			var digitals17=reader.GetDigitalChannel(16);
			
						
			Assert.That(timeLine[0],Is.EqualTo(0).Within(0.01));
			Assert.That(timeLine[1],Is.EqualTo(500).Within(0.01));
			Assert.That(timeLine[2],Is.EqualTo(1000).Within(0.01));
			
			Assert.That(analogs1[0],Is.EqualTo(0).Within(0.01));
			Assert.That(analogs1[1],Is.EqualTo(1.0).Within(0.01));
			Assert.That(analogs1[2],Is.EqualTo(-1.0).Within(0.01));
			
			Assert.That(analogs2[0],Is.EqualTo(0).Within(0.01));
			Assert.That(analogs2[1],Is.EqualTo(2.0).Within(0.01));
			Assert.That(analogs2[2],Is.EqualTo(2.0).Within(0.01));
			
			Assert.That(analogs3[0],Is.EqualTo(0).Within(0.01));
			Assert.That(analogs3[1],Is.EqualTo(3.0).Within(0.01));
			Assert.That(analogs3[2],Is.EqualTo(-3.5).Within(0.01));
			
			Assert.That(digitals1[0],Is.EqualTo(true));
			Assert.That(digitals1[1],Is.EqualTo(false));
			Assert.That(digitals1[2],Is.EqualTo(false));
			
			Assert.That(digitals5[0],Is.EqualTo(true));
			Assert.That(digitals5[1],Is.EqualTo(false));
			Assert.That(digitals5[2],Is.EqualTo(true));
			
			Assert.That(digitals17[0],Is.EqualTo(true));
			Assert.That(digitals17[1],Is.EqualTo(false));
			Assert.That(digitals17[2],Is.EqualTo(true));
			
			Assert.That(reader.Configuration.startTime,  Is.EqualTo(new DateTime(1234567890)));
			Assert.That(reader.Configuration.triggerTime,Is.EqualTo(new DateTime(1234569000)));
			
		}
						
		[Test, Order(1)]
		public void SaveToFileAsciiTest()
		{
			var writer=this.GetWriterToTest();
			writer.SaveToFile(fullPathAsciiOne,DataFileType.ASCII);			
			this.ReaderAsserts(fullPathAsciiOne);			
		}
		
		[Test, Order(2)]
		public void SaveToFileBinaryTest()
		{
			const string fullPath=@"D:\YandexDisk\Oscillogram\AutoCreated\bin.cfg";
			var writer=this.GetWriterToTest();
			writer.SaveToFile(fullPath,DataFileType.Binary);			
			
			this.ReaderAsserts(fullPath);			
		}
		
		[Test, Order(3)]
		public void CreateWriterFromReaderTest()
		{
			const string fullPath=@"D:\YandexDisk\Oscillogram\AutoCreated\bin.cfg";
			var reader=new RecordReader(fullPath);
			var writer=new RecordWriter(reader);			
			
			writer.SaveToFile(fullPathAsciiTwo, DataFileType.ASCII);

			//TODO дописать тест - открыть сконфигурированный и проверить равенство исходному бинарному			
		}
	}
}

#endif