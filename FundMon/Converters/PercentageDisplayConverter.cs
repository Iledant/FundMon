using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FundMon.Converters;

public class PercentageDisplayConverter : IValueConverter
{
    private static readonly CultureInfo ci = new("fr-FR");

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            if (double.IsNaN(doubleValue))
                return "-";
            return doubleValue.ToString("P2", ci);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
