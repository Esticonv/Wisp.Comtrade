namespace Wisp.Comtrade;

/// <summary>
/// Description of GlobalSettings.
/// </summary>
public static class GlobalSettings
{
	internal const char Comma=',';
	internal const char WhiteSpace=' ';
	internal const string NewLine="\r\n";
	internal const string DateTimeFormatForWrite = "dd/MM/yyyy,HH:mm:ss.fffffff";
	internal const string DateTimeFormatForParseMicroSecond =	"d/M/yyyy,HH:mm:ss.ffffff";
	internal const string DateTimeFormatForParseNanoSecond =	"d/M/yyyy,HH:mm:ss.fffffff";		
	/// <summary>
	/// Extentions of COMTRADE files
	/// </summary>
	public const string ExtentionsForFileDialogFilter="(*.cfg;*.dat;*.cff)|*.cfg;*.dat;*.cff";
	internal const string ExtentionCFG=".cfg";
	internal const string ExtentionDAT=".dat";
	internal const string ExtentionCFF=".cff";	
}
