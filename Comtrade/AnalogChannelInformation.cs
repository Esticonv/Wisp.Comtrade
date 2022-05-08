using System;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Description of AnalogChannelInformation.
	/// </summary>
	public class AnalogChannelInformation
	{		
		int index=0;
		
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		public int Index{
			get{
				return index;
			}
			internal set{
				this.index=value;
			}
		}		
		
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public string name=string.Empty;
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public string phase=string.Empty;
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public string circuitComponent=string.Empty;
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public string units="NONE";
		
		internal double a=1.0;
				
		internal double b=0;
		
		readonly internal double skew=0;
		
		
		double min=float.MinValue;
		
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		public double Min
		{
			get{
				return this.min;
			}
			internal set{
				this.min=value;
			}			
		}		
		
		double max=float.MaxValue;
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		public double Max
		{
			get{
				return this.max;
			}
			internal set{
				this.max=value;
			}			
		}
		
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public double primary=1.0;
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public double secondary=1.0;
		
		readonly internal bool isPrimary=true;	
		
		/// <summary>
		/// Constructor
		/// </summary>
		public AnalogChannelInformation(string name, string phase)
		{
			this.name=name;
			this.phase=phase;
		}
		
		internal AnalogChannelInformation(string analogLine)
		{
			var values=analogLine.Split(GlobalSettings.commaDelimiter);
			
			this.index=Convert.ToInt32(values[0].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.name=values[1].Trim(GlobalSettings.whiteSpace);
			this.phase=values[2].Trim(GlobalSettings.whiteSpace);
			this.circuitComponent=values[3].Trim(GlobalSettings.whiteSpace);
			this.units=values[4].Trim(GlobalSettings.whiteSpace);
			this.a=Convert.ToDouble(values[5].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.b=Convert.ToDouble(values[6].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);			
			this.skew=Convert.ToDouble(values[7].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.min=Convert.ToDouble(values[8].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.max=Convert.ToDouble(values[9].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.primary=Convert.ToDouble(values[10].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.secondary=Convert.ToDouble(values[11].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			
			string isPrimaryText=values[12].Trim(GlobalSettings.whiteSpace);
			if(isPrimaryText=="S" || isPrimaryText=="s"){
				this.isPrimary=false;
			}						
		}
		
		internal string ToCFGString()
		{
			return this.Index.ToString()+GlobalSettings.commaDelimiter+
					this.name+GlobalSettings.commaDelimiter+
					this.phase+GlobalSettings.commaDelimiter+
					this.circuitComponent+GlobalSettings.commaDelimiter+
					this.units+GlobalSettings.commaDelimiter+
					this.a.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.b.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.skew.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.min.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.max.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.primary.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					this.secondary.ToString(System.Globalization.CultureInfo.InvariantCulture)+GlobalSettings.commaDelimiter+
					(this.isPrimary ? "P" : "S");
		}
	}
}
