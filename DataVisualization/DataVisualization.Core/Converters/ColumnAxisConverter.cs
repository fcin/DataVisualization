using DataVisualization.Core.ViewModels.DataLoading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    public class ColumnAxisConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)values[0];
            if (index < 0 || !(values[1] is List<SelectableAxes> selectableAxes))
                return null;

            return selectableAxes[index].MyAxisTypes;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
