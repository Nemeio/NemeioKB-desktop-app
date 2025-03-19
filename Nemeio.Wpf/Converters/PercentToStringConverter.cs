using System;
using System.Globalization;
using System.Windows.Data;

namespace Nemeio.Wpf.Converters
{
    public class PercentToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = string.Empty;

            if (value != null)
            {
                var doubleValue = (double)value;
                var roundedValue = doubleValue / 100;
                result = roundedValue.ToString("P0");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
