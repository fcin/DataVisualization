using DataVisualization.Core.ViewModels.DataLoading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    /// <summary>
    /// Returns all possible column types for combobox.
    /// </summary>
    public class ColumnTypesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null)
                return null;

            var selectedIndex = (int)values[0];
            if (selectedIndex != -1)
            {
                var types = (List<SelectableTypes>)values[1];
                return types[selectedIndex].MyColumnTypes;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
