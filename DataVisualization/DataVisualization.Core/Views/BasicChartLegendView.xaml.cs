using Caliburn.Micro;
using DataVisualization.Core.ViewModels;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Series = DataVisualization.Models.Series;

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

        private readonly IWindowManager _windowManager;
        private readonly Data _data;
        private Action<Series> _onSeriesChanged;

        public BasicChartLegendView(IWindowManager windowManager, Data data, Action<Series> onSeriesChanged)
        {
            _windowManager = windowManager;
            _data = data;
            _onSeriesChanged = onSeriesChanged;

            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSeriesClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            var border = (Border)sender;
            var series = _data.Series.First(d => d.Name == border.Tag.ToString());

            var result = _windowManager.ShowDialog(new SeriesPropertiesViewModel(series));
            if(result.HasValue && result.Value)
                _onSeriesChanged(series);
        }
    }
}
