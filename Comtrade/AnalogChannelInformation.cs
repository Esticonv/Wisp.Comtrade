using System;
using System.Globalization;

namespace Wisp.Comtrade;

public class AnalogChannelInformation
{
    /// <summary>
    ///     Parameter 'PS'
    ///     Indicates whether the converted data is in primary (P) or secondary (S) values
    /// </summary>
    public readonly bool IsPrimary = true;

    /// <summary>
    ///     Parameter 'primary'
    ///     Channel voltage or current transformer ratio primary factor
    /// </summary>
    public readonly double Primary = 1.0;

    /// <summary>
    ///     Parameter 'secondary'
    ///     Channel voltage or current transformer ratio secondary factor
    /// </summary>
    public readonly double Secondary = 1.0;

    public AnalogChannelInformation(string name, string phase)
    {
        Name = name;
        Phase = phase;
    }

    public AnalogChannelInformation(string analogLine)
    {
        //TODO: Check if line length == 13;
        var values = analogLine.Split(GlobalSettings.Comma);

        Index = Convert.ToInt32(values[0].Trim(), CultureInfo.InvariantCulture);
        Name = values[1].Trim();
        Phase = values[2].Trim();
        CircuitComponent = values[3].Trim();
        Units = values[4].Trim();
        MultiplierA = Convert.ToDouble(values[5].Trim(), CultureInfo.InvariantCulture);
        MultiplierB = Convert.ToDouble(values[6].Trim(), CultureInfo.InvariantCulture);
        Skew = Convert.ToDouble(values[7].Trim(), CultureInfo.InvariantCulture);
        Min = Convert.ToDouble(values[8].Trim(), CultureInfo.InvariantCulture);
        Max = Convert.ToDouble(values[9].Trim(), CultureInfo.InvariantCulture);
        Primary = Convert.ToDouble(values[10].Trim(), CultureInfo.InvariantCulture);
        Secondary = Convert.ToDouble(values[11].Trim(), CultureInfo.InvariantCulture);

        var isPrimaryText = values[12].Trim();

        if (isPrimaryText.ToLower().Equals("s")) {
            IsPrimary = false;
        }
    }

    /// <summary>
    ///     Parameter 'An'
    ///     Analog channel index number
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    ///     Parameter 'ch_id'
    ///     Channel identifier
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Parameter 'ph'
    ///     Channel phase identification
    /// </summary>
    public string Phase { get; }

    /// <summary>
    ///     Parameter 'ccbm'
    ///     Circuit component being monitored
    /// </summary>
    public string CircuitComponent { get; } = string.Empty;

    /// <summary>
    ///     Parameter 'uu'
    ///     Channel units (when data has been converted using channel conversion factor)
    /// </summary>
    public string Units { get; } = "NONE";

    /// <summary>
    ///     Parameter 'a'
    ///     Channel multiplier (channel conversion factor)
    /// </summary>
    public double MultiplierA { get; set; } = 1.0;

    /// <summary>
    ///     Parameter 'b'
    ///     Channel offset adder (channel conversion factor)
    /// </summary>
    public double MultiplierB { get; set; }

    /// <summary>
    ///     Parameter 'skew'
    ///     Channel time skew (in microseconds) from the start of sample period.
    /// </summary>
    public double Skew { get; }

    /// <summary>
    ///     Parameter 'min'
    ///     Channel minimum data range value
    /// </summary>
    public double Min { get; internal set; } = float.MinValue;

    /// <summary>
    ///     Parameter 'max'
    ///     Channel maximum data range value
    /// </summary>
    public double Max { get; internal set; } = float.MaxValue;

    internal string ToCFGString()
    {
        var cfgValues = new[] {
            Index.ToString(),
            Name,
            Phase,
            CircuitComponent,
            Units,
            MultiplierA.ToString(CultureInfo.InvariantCulture),
            MultiplierB.ToString(CultureInfo.InvariantCulture),
            Skew.ToString(CultureInfo.InvariantCulture),
            Min.ToString(CultureInfo.InvariantCulture),
            Max.ToString(CultureInfo.InvariantCulture),
            Primary.ToString(CultureInfo.InvariantCulture),
            Secondary.ToString(CultureInfo.InvariantCulture),
            IsPrimary ? "P" : "S"
        };

        return string.Join(GlobalSettings.Comma.ToString(), cfgValues);
    }
}
