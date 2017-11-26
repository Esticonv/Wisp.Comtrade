/*
 * Created by SharpDevelop.
 * User: borisov
 * Date: 23.05.2017
 * Time: 13:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		internal const string dateTimeFormat="dd/MM/yyyy,HH:mm:ss.ffffff";
		/// <summary>
		/// Extentions of COMTRADE files
		/// </summary>
		public const string extentionsForFileDialogFilter="(*.cfg;*.dat;*.cff)|*.cfg;*.dat;*.cff";
		internal const string extentionCFG=".cfg";
		internal const string extentionDAT=".dat";
		internal const string extentionCFF=".cff";
		
	}
}
