using System;
using Wisp.Comtrade.Models;

namespace Wisp.Comtrade.Utils;

internal static class DataFileTypeConverter
{
    internal static DataFileType Get(string text)
    {
        return text.ToLowerInvariant() switch
        {
            "ascii" => DataFileType.ASCII,
            "binary" => DataFileType.Binary,
            "binary32" => DataFileType.Binary32,
            "float32" => DataFileType.Float32,
            _ => throw new InvalidOperationException("Undefined *.dat file format")
        };
    }
}
