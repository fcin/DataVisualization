using DataVisualization.Core.ViewModels.SettingsWindow;
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
using System.Windows.Shapes;

namespace DataVisualization.Core.Views.SettingsWindow
{
    /// <summary>
    /// Interaction logic for GlobalSettingsView.xaml
    /// </summary>
    public partial class GlobalSettingsView : Window
    {
        public GlobalSettingsView()
        {
            InitializeComponent();
        }

        private void SelectedNode(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((GlobalSettingsViewModel)DataContext).NodeSelected((TreeNode)e.NewValue);
        }
    }
}
