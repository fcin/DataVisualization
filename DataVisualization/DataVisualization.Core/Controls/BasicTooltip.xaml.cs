using System;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using DataVisualization.Models;

namespace DataVisualization.Core.Controls
{
    /// <summary>
    /// Interaction logic for BasicTooltip.xaml
    /// </summary>
    public partial class BasicTooltip : UserControl, IChartTooltip
    {
        private readonly Func<double, string> _titleFormatter;
        public string FormattedTitle => _data.SharedValue != null ? _titleFormatter(_data.SharedValue.Value) : "No value";

        public BasicTooltip(Func<double, string> titleFormatter)
        {
            _titleFormatter = titleFormatter;

            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private TooltipData _data;
        public TooltipData Data
        {
            get
            {
                OnPropertyChanged(nameof(FormattedTitle));
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
