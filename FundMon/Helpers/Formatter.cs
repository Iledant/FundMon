using Microsoft.UI.Xaml.Media;
using System.Globalization;

namespace FundMon.Helpers;

public static class Formatter
{
    public static readonly CultureInfo Culture = new("fr-FR", false);
    
    public static string Currency(double value)
    {
        if (double.IsNaN(value))
            return "-";
        return value.ToString("C", Culture);
    }

    public static string Percentage(double value)
    {
        if (double.IsNaN(value))
            return "-";
        return value.ToString("P1", Culture);
    }

    public static Brush PerfToColor(double value)
    {
        if (value < 0)
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 0, 0));
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 40, 0));
    }

    public static string EvolutionGlyph(double value)
    {
        if (double.IsNaN(value))
            return "\xEDD6"; // ChevronRightSmall
        if (value > 0)
            return "\xEDD7"; // ChevronUpSmall
        return "\xEDD8"; // ChevronDownSmall
    }
}
