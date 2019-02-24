using DataVisualization.Core.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for SeriesPropertiesView.xaml
    /// </summary>
    public partial class SeriesPropertiesView : Window
    {
        public SeriesPropertiesView()
        {
            InitializeComponent();
        }

        private void TryUpdateItemValue(object sender, TextCompositionEventArgs e)
        {
            if (!(sender is TextBox textbox))
                throw new InvalidOperationException();

            var text = textbox.Text.Insert(textbox.SelectionStart, e.Text);
            if (double.TryParse(text, out var value))
            {
                e.Handled = false;
                return;
            }

            e.Handled = true;
        }

        private void TryPasteItemValue(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!double.TryParse(text, out var _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
