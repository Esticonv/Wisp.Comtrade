using Wisp.Comtrade.Models;

namespace Wisp.Comtrade.Utils;

internal static class ComtradeVersionConverter
{
    internal static ComtradeVersion Get(string? versionText)
    {
        return versionText switch
        {
            null => ComtradeVersion.V1991,
            "1991" => ComtradeVersion.V1991,
            "1999" => ComtradeVersion.V1999,
            "2013" => ComtradeVersion.V2013,
            _ => ComtradeVersion.V1991
        };
    }
}
