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
            var context = ((FrameworkElement)e.OriginalSource)?.DataContext;
            if(context != null && context is DataConfiguration selectedConfig)
                ((DataBrowserViewModel)DataContext).OpenConfiguration(selectedConfig);
        }
    }
}
