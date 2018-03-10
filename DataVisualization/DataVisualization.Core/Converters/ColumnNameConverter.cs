using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    public class ColumnNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values?[0] is ObservableCollection<Tuple<string, string>>) || values[1] == null)
                return null;

            var columns = (ObservableCollection<Tuple<string, string>>)values[0];
            var selectedColumnIndex = (int)values[1];

            return selectedColumnIndex == -1 ? null : columns[selectedColumnIndex].Item1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("Test");
            return new []{ value };
        }
    }
}
