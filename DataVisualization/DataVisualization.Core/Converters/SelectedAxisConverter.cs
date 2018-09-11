using DataVisualization.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    public class SelectedAxisConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)values[0];
            if (index < 0 || !(values[1] is List<SelectableAxes> selectableAxes))
                return null;

            return selectableAxes[index].SelectedAxis;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
