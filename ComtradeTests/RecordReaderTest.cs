/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 30.05.2017
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class RecordReaderTest
	{
		const string rootYandexDiskDirectory = RecordWriterTest.rootYandexDiskDirectory;

		/// <summary>
		/// Success only on maintainer machine 
		/// </summary>
		[Test]		
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
		
		[Test]
		public void TestNotSupportedExtentions()
		{					
			Assert.Throws<InvalidOperationException> (() => new RecordReader("notComtradeExtentions.trr"));
		}
	}
}

