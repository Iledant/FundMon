using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FundMon.Converters;

public class FrenchDoubleConverter : IValueConverter
{
    private static readonly CultureInfo ci = new("fr-FR");
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return doubleValue.ToString("G", ci);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string s)
        {
            if (double.TryParse(s, NumberStyles.AllowDecimalPoint, ci, out double doubleValue))
                return doubleValue;
        }
        return DependencyProperty.UnsetValue;
    }
}
