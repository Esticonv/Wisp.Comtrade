/*
 * Created by SharpDevelop.
 * User: EstiMain
 * Date: 30.05.2017
 * Time: 20:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
#if TEST
using System;
using NUnit.Framework;

namespace Wisp.Comtrade
{
	[TestFixture]
	internal class RecordReaderTest
	{
		/// <summary>
		/// Success only on maintainer machine 
		/// </summary>
		[Test]		
		public void TestOpenFile()
		{	
			RecordReader record;
			record=new RecordReader(@"D:\YandexDisk\Oscillogram\Undefined_A\1.dat");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(@"D:\YandexDisk\Oscillogram\LossOfSyncronism_B\ALAR.DAT");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(@"D:\YandexDisk\Oscillogram\Ground Fault_B\3.cFg");
			record.GetTimeLine();
			record.GetAnalogPrimaryChannel(0);
			record.GetDigitalChannel(0);
			
			record=new RecordReader(@"D:\YandexDisk\Oscillogram\Ground Fault_B\3.cfg");	
			
			record=new RecordReader(@"D:\YandexDisk\Oscillogram\Sepam_StopingGenerator_B\1.DAT");
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
#endif
