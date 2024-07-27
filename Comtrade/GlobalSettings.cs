using System;

namespace Wisp.Comtrade;

/// <summary>
/// Description of GlobalSettings.
/// </summary>
public static class GlobalSettings
{
	internal const char Comma=',';
	internal const char WhiteSpace=' ';
	/// <summary>
	/// 'CRLF' must be used by standard as separator
	/// </summary>
    internal const string NewLineWindows="\r\n";
    private const string NewLineUnix = "\n";
    /// <summary>
    /// 'CRLF' must be used by standard as separator, but it occurs in different ways
    /// </summary>
    internal static readonly string[] NewLines=[GlobalSettings.NewLineWindows, GlobalSettings.NewLineUnix];
	internal const string DateTimeFormatForWrite = "dd/MM/yyyy,HH:mm:ss.fffffff";
	internal const string DateTimeFormatForParseMicroSecond =	"d/M/yyyy,HH:mm:ss.ffffff";
	internal const string DateTimeFormatForParseNanoSecond =	"d/M/yyyy,HH:mm:ss.fffffff";		
	/// <summary>
	/// Extensions of COMTRADE files
	/// </summary>
	public const string ExtensionsForFileDialogFilter="(*.cfg;*.dat;*.cff)|*.cfg;*.dat;*.cff";
    public const string ExtensionCFG=".cfg";
    public const string ExtensionDAT=".dat";
    public const string ExtensionCFF=".cff";	
}
