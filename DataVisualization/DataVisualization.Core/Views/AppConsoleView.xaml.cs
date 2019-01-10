using DataVisualization.Models;
using System.Windows;
using System.Windows.Controls;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for AppConsoleView.xaml
    /// </summary>
    public partial class AppConsoleView : UserControl
    {
        public AppConsoleView()
        {
            InitializeComponent();
        }

        private void CopyLog(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem))
                return;
            if (!(menuItem.DataContext is AppConsoleLog log))
                return;

            var formatted = $"{log.LoggedAt:yyyy-MM-dd HH:mm:ss}: {log.LoggedAtLocal}, {log.Message}";
            Clipboard.SetText(formatted, TextDataFormat.UnicodeText);
        }
    }
}
