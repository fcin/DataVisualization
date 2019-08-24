using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DataVisualization.Core.Converters
{
    public class ColorHexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is string hexColor))
                throw new InvalidCastException(nameof(value));

            var color = (Color) (ColorConverter.ConvertFromString(hexColor) ?? Colors.Black);
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
