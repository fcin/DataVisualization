using DataVisualization.Core.ViewModels;
using DataVisualization.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for DataBrowserView.xaml
    /// </summary>
    public partial class DataBrowserView : UserControl
    {
        public DataBrowserView()
        {
            InitializeComponent();
        }

        private void OpenConfig(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListBoxItem item))
                return;

            if (!(item.DataContext is DataConfiguration selectedConfig))
                return;

            ((DataBrowserViewModel)DataContext).OpenConfiguration(selectedConfig);
        }

        private void OnOpenConfiguration(object sender, RoutedEventArgs e)
        {
            var selectedItem = GetClickedListViewItem(sender);
            ((DataBrowserViewModel)DataContext).OpenConfiguration(selectedItem);
        }

        private async void OnDeleteConfiguration(object sender, RoutedEventArgs e)
        {
            var selectedItem = GetClickedListViewItem(sender);
            await ((DataBrowserViewModel)DataContext).DeleteConfigurationAsync(selectedItem);
        }

        private DataConfiguration GetClickedListViewItem(object sender)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is ContextMenu parentContextMenu)
                {
                    var listView = parentContextMenu.PlacementTarget as ListView;
                    var selectedItem = (DataConfiguration)listView.SelectedItem;
                    return selectedItem;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
