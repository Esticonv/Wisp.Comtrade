using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Wisp.Comtrade
{
	[TestClass]
	public class RecordReaderTest
	{
		const string rootYandexDiskDirectory = RecordWriterTest.rootYandexDiskDirectory;

		/// <summary>
		/// Success only on maintainer machine 
		/// </summary>
		[TestMethod]		
		public void TestOpenFile()
		{	
			RecordReader record;
			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\Undefined_A\1.dat");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\LossOfSyncronism_B\ALAR.DAT");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\Ground Fault_B\3.cFg");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\Ground Fault_B\3.cfg");	
			
			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\Sepam_StopingGenerator_B\1.DAT");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);	

			record=new RecordReader(rootYandexDiskDirectory + @"Oscillogram\Undefined_2013_B32\000.DAT");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);			
		}
		
		[TestMethod]
		public void TestNotSupportedExtentions()
		{
			Assert.ThrowsException<InvalidOperationException>(() => new RecordReader("notComtradeExtentions.trr"));
		}
	}
}

