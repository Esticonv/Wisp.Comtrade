/*
 * Created by SharpDevelop.
 * User: borisov
 * Date: 23.05.2017
 * Time: 15:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Wisp.Comtrade
{
	/// <summary>
	/// Information for digital channel
	/// </summary>
	public class DigitalChannelInformation
	{
		/// <summary>
		/// According STD for COMTRADE
		/// </summary>
		readonly public int index=0;
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
		readonly public bool normalState=false;
		
		internal DigitalChannelInformation(string digitalLine)
		{
			var values=digitalLine.Split(GlobalSettings.commaDelimiter);
			
			this.index=Convert.ToInt32(values[0].Trim(GlobalSettings.whiteSpace), System.Globalization.CultureInfo.InvariantCulture);
			this.name=values[1].Trim(GlobalSettings.whiteSpace);
			this.phase=values[2].Trim(GlobalSettings.whiteSpace);
			this.circuitComponent=values[3].Trim(GlobalSettings.whiteSpace);
			if(values.Length>4){//some files not include this part of line
				this.normalState=Convert.ToBoolean(Convert.ToInt32(values[4].Trim(GlobalSettings.whiteSpace),
				                                                   System.Globalization.CultureInfo.InvariantCulture));
			}
		}
	}
}
