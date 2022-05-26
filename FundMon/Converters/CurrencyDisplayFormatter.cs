using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FundMon.Converters;

public class CurrencyDisplayConverter : IValueConverter
{
    private static readonly CultureInfo ci = new("fr-FR");

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return doubleValue.ToString("C", ci);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
