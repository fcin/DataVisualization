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
using DataVisualization.Core.ViewModels;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for CodeEditorView.xaml
    /// </summary>
    public partial class CodeEditorView : UserControl
    {
        public CodeEditorView()
        {
            InitializeComponent();
            Loaded += Onloaded;
        }

        private void Onloaded(object sender, RoutedEventArgs e)
        {
            ((CodeEditorViewModel)DataContext).TextEditor = Editor;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.PrimaryDevice.IsKeyDown(Key.LeftCtrl) && Keyboard.PrimaryDevice.IsKeyDown(Key.S))
            {
                ((CodeEditorViewModel) DataContext).OnSave();
            }
        }
    }
}
