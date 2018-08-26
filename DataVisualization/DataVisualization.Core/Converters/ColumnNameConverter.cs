using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using DataVisualization.Models;

namespace DataVisualization.Core.Converters
{
    public class ColumnNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values?[0] is ObservableCollection<GridColumn>) || values[1] == null)
                return null;

            var columns = (ObservableCollection<GridColumn>)values[0];
            var selectedColumnIndex = (int)values[1];

            return selectedColumnIndex == -1 ? null : columns[selectedColumnIndex].Name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("Test");
            return new []{ value };
        }
    }
}
