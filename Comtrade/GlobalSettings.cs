using System;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Description of GlobalSettings.
	/// </summary>
	public static class GlobalSettings
	{
		internal const char commaDelimiter=',';
		internal const char whiteSpace=' ';
		internal const string newLine="\r\n";
		internal const string dateTimeFormatForWrite = "dd/MM/yyyy,HH:mm:ss.fffffff";
		internal const string dateTimeFormatForParseMicroSecond =	"d/M/yyyy,HH:mm:ss.ffffff";
		internal const string dateTimeFormatForParseNanoSecond =	"d/M/yyyy,HH:mm:ss.fffffff";		
		/// <summary>
		/// Extentions of COMTRADE files
		/// </summary>
		public const string extentionsForFileDialogFilter="(*.cfg;*.dat;*.cff)|*.cfg;*.dat;*.cff";
		internal const string extentionCFG=".cfg";
		internal const string extentionDAT=".dat";
		internal const string extentionCFF=".cff";
		
	}
}
