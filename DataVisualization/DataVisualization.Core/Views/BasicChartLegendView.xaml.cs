using LiveCharts.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for BasicChartLegendView.xaml
    /// </summary>
    public partial class BasicChartLegendView : UserControl, IChartLegend
    {
        private List<SeriesViewModel> _series = new List<SeriesViewModel>();
        public List<SeriesViewModel> Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged("Series");
            }
        }

        public BasicChartLegendView()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
