using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    public class ColumnIgnoreConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)values[0];
            if (index < 0 || !(values[1] is List<bool> boolList))
                return null;

            return boolList[index];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
