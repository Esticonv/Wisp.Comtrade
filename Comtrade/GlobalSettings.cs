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
	/// Extensions of COMTRADE files
	/// </summary>
	public const string ExtensionsForFileDialogFilter="(*.cfg;*.dat;*.cff)|*.cfg;*.dat;*.cff";
    public const string ExtensionCFG=".cfg";
    public const string ExtensionDAT=".dat";
    public const string ExtensionCFF=".cff";	
}
