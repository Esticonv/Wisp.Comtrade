using System;

namespace Wisp.Comtrade
{
	
	/// <summary>
	/// Possible comtrade versions
	/// </summary>	
	internal enum ComtradeVersion
	{
		V1991=0,
		V1999=1,
		V2013=2,
	};
	
	internal static class ComtradeVersionConverter
	{
		internal static ComtradeVersion Get(string text)
		{
			if(text==null)return ComtradeVersion.V1991;
			if(text=="1991")return ComtradeVersion.V1991;
			if(text=="1999")return ComtradeVersion.V1999;
			if(text=="2013")return ComtradeVersion.V2013;
			return ComtradeVersion.V1991;
		}
	}
	
	/// <summary>
	/// Data file type
	/// </summary>
	public enum DataFileType
	{	
		/// <summary>
		/// </summary>		
		Undefined=0,
		/// <summary>
		/// </summary>
		ASCII,
		/// <summary>
		/// </summary>
		Binary,
		/// <summary>
		/// </summary>
		Binary32,
		/// <summary>
		/// </summary>
		Float32
	}
	
	internal static class DataFileTypeConverter
	{
		internal static DataFileType Get(string text)
		{
			text=text.ToLowerInvariant();
			if(text=="ascii")return DataFileType.ASCII;
			if(text=="binary")return DataFileType.Binary;
			if(text=="binary32")return DataFileType.Binary32;
			if(text=="float32")return DataFileType.Float32;
			throw new InvalidOperationException("Undefined *.dat file format");
		}
	}
		
	
}
