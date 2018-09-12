using DataVisualization.Core.ViewModels;
using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            var source = (FrameworkElement)e.OriginalSource;
            while (source != null && !(source.DataContext is DataConfiguration))
                source = (FrameworkElement)VisualTreeHelper.GetParent(source);

            if (source == null)
                throw new InvalidOperationException(nameof(source));

            var selectedConfig = (DataConfiguration)source.DataContext;
            ((DataBrowserViewModel)DataContext).OpenConfiguration(selectedConfig);
        }
    }
}
