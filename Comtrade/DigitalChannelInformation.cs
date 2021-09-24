using System;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Information for digital channel
	/// </summary>
	public class DigitalChannelInformation
	{
		public int Index { get; internal set; }		
	
		readonly public string name=string.Empty;
	
		readonly public string phase=string.Empty;

		readonly public string circuitComponent=string.Empty;
		
		readonly public bool normalState=false;
		
		public DigitalChannelInformation(string name, string phase)
		{
			this.name=name;
			this.phase=phase;
		}
		
		internal DigitalChannelInformation(string digitalLine)
		{
			var values=digitalLine.Split(GlobalSettings.commaDelimiter);
			
			this.Index=Convert.ToInt32(values[0].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.name=values[1].Trim(GlobalSettings.whiteSpace);
			this.phase=values[2].Trim(GlobalSettings.whiteSpace);
			this.circuitComponent=values[3].Trim(GlobalSettings.whiteSpace);
			if(values.Length>4){//some files not include this part of line
				this.normalState=Convert.ToBoolean(Convert.ToInt32(values[4].Trim(GlobalSettings.whiteSpace),
				                                                   System.Globalization.CultureInfo.InvariantCulture));
			}
		}
		
		internal string ToCFGString()
		{
			return this.Index.ToString()+GlobalSettings.commaDelimiter+
					this.name+GlobalSettings.commaDelimiter+
					this.phase+GlobalSettings.commaDelimiter+
					this.circuitComponent+GlobalSettings.commaDelimiter+
					(this.normalState ? "1" : "0");
		}
	}
}
