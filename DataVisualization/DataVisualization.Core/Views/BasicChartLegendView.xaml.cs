using Caliburn.Micro;
using DataVisualization.Core.ViewModels;
using DataVisualization.Services;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Action = System.Action;
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
        private Action<Series> _onSeriesChanged;

        public BasicChartLegendView(IWindowManager windowManager, Action<Series> onSeriesChanged)
        {
            _windowManager = windowManager;
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
            var series = new DataService().GetSeriesByName(border.Tag.ToString());

            var result = _windowManager.ShowDialog(new SeriesPropertiesViewModel(series));
            if(result.HasValue && result.Value)
                _onSeriesChanged(series);
        }
    }
}
