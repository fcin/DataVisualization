using DataVisualization.Core.ViewModels.DataLoading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DataVisualization.Core.Converters
{
    /// <summary>
    /// Returns currently selected item in combobox.
    /// </summary>
    public class SelectedComboboxItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values == null)
                return null;

            var selectedIndex = (int)values[0];

            if (selectedIndex != -1)
            {
                var items = (List<SelectableTypes>)values[1];
                return items[selectedIndex].SelectedType;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
