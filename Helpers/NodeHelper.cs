using System.Runtime;
using System.Text.RegularExpressions;

namespace MauiBench.Helpers
{
    public class NodeHelper
    {
        public static string GetProcessNode(string gpuCore)
        {
            return gpuCore switch
            {
                _ when gpuCore.Contains("TU") => "TSMC 12nm",     // Turing - RTX 16xx, RTX 20xx
                _ when gpuCore.Contains("GA") => "Samsung 8nm",   // Ampere - RTX 30xx
                _ when gpuCore.Contains("AD") => "TSMC 4nm",      // Lovelace - RTX 40xx
                _ when gpuCore.Contains("GP") => "TSMC 16nm",     // Pascal - GTX 10xx
                _ when gpuCore.Contains("GM") => "TSMC 28nm",     // Maxwell - GTX 9xx
                _ when gpuCore.Contains("GB") => "TSMC 4nm",      // Blackwell - RTX 50xx
                _ => "Unknown"
            };
        }

        public static string GetProcessNodeAMD(string gpuName)
        {
            gpuName = gpuName.ToUpperInvariant();

            if (Regex.IsMatch(gpuName, @"RX\s+(4\d{2}|5[6-9]\d)"))
                return "GloFo 14nm";

            if (Regex.IsMatch(gpuName, @"VEGA"))
                return "GloFo 14nm";

            if (Regex.IsMatch(gpuName, @"RX\s+5\d{3}"))
                return "TSMC 7nm";

            if (Regex.IsMatch(gpuName, @"RX\s+6\d{3}"))
                return "TSMC 7nm";

            if (Regex.IsMatch(gpuName, @"RX\s+7\d{3}"))
                return "TSMC 5nm";

            if (Regex.IsMatch(gpuName, @"RX\s+9\d{3}"))
                return "TSMC 4nm";

            return "Unknown";
        }
    }
}