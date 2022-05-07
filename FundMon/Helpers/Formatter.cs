using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.Helpers;

public static class Formatter
{
    private static readonly CultureInfo culture = new("fr-FR", false);
    
    public static string PerformanceValueFormatter(double value)
    {
        if (double.IsNaN(value))
            return "-";
        return value.ToString("C", culture);
    }
}
