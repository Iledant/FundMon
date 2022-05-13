using Microsoft.UI.Xaml.Media;
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

    public static Brush PerfToColor(double value)
    {
        if (value < 0)
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 0, 0));
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 40, 0));
    }
}
