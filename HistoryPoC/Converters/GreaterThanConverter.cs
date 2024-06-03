using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HistoryPoC.Converters;

public class GreaterThanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        if (double.TryParse(value.ToString(), out double valueToCompare) &&
            double.TryParse(parameter.ToString(), out double referenceValue))
        {
            return valueToCompare > referenceValue;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}